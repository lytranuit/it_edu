<template>
    <div class="row clearfix">
        <div class="col-12">
            <form method="POST" id="form">
                <AlertError :message="messageError" v-if="messageError" />
                <AlertSuccess :message="messageSuccess" v-if="messageSuccess" />
                <section class="card card-fluid">

                    <div class="d-inline-block w-100">
                        <div class="card-header"><a href="#" class="btn btn-sm btn-success" data-toggle='modal'
                                data-animation='bounce' data-target='.modal-pass'>Đổi mật khẩu</a>

                            <button type="submit" class="btn btn-sm btn-primary float-right"
                                @click.prevent="submit()">Save</button>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-8">
                                <input type="hidden" name="id" v-model="data.id" required />
                                <div class="form-group row">
                                    <b class="col-12 col-lg-2 col-form-label">Email:<i class="text-danger">*</i></b>
                                    <div class="col-12 col-lg-4 pt-1">
                                        <input class="form-control form-control-sm" type="text" name="email"
                                            placeholder="Email" v-model="data.email" required />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <b class="col-12 col-lg-2 col-form-label">Họ và tên:<i class="text-danger">*</i></b>
                                    <div class="col-12 col-lg-4 pt-1">
                                        <input class="form-control form-control-sm" type="text" name="fullName" required=""
                                            placeholder="FullName" v-model="data.fullName" />
                                    </div>
                                    <b class="col-12 col-lg-2 col-form-label">Nhóm:</b>
                                    <div class="col-lg-4 pt-1">
                                        <treeselect :options="roles" multiple v-model="data.roles"></treeselect>
                                        <select name="roles[]" v-model="data.roles" multiple class="d-none">
                                            <option v-for="option in data.roles" :key="option" :value="option">
                                            </option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group row">
                                    <div class="col-12">
                                        <div class="card no-shadow border">
                                            <div class="card-header">
                                                Hình đại diện
                                            </div>
                                            <div class="card-body text-center">
                                                <img :src="data.image_url" class="image_feature"
                                                    style="max-height: 100px; max-width: 100%; cursor: pointer;"
                                                    @click="fileDialog = true">
                                                <input class="form-control form-control-sm" type="hidden" name="image_url"
                                                    required="" :value="data.image_url">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            </form>
        </div>
        <div class="modal fade modal-pass" tabindex="-1" role="dialog">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <form method="POST" id="form-pass">
                        <div class="modal-header">
                            <h5 class="modal-title mt-0" id="myLargeModalLabel">Đổi mật khẩu</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                        </div>
                        <div class="modal-body">
                            <input type="hidden" name="id" :value="data.id" />
                            <div class="row justify-content-md-center">
                                <div class="col-md-4">
                                    <div class="form-floating mt-2">
                                        <input type="password" id="password" class="form-control" name="NewPassword"
                                            minlength="6" required="" placeholder="Mật khẩu mới" autocomplete="off" />
                                    </div>
                                    <div class="form-floating mt-2">
                                        <input type="password" class="form-control" name="ConfirmPassword" minlength="6"
                                            data-rule-equalTo="#password" required="" placeholder="Nhập lại mật khẩu mới"
                                            autocomplete="off">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="model-fotter">
                            <div class="row justify-content-md-center m-2">
                                <div class="col-12 text-center">
                                    <button type="submit" class="btn btn-sm btn-primary" style="width:200px"
                                        @click.prevent="change_pass()">Cập nhật
                                        mật khẩu</button>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        <Dialog v-model:visible="fileDialog" header="File Manager" :modal="true" style="width:50vw">
            <FileManagerComponent id="file-manager" :ajaxSettings="{
                url: '/FileManager/FileManager/FileOperations',
                getImageUrl: '/FileManager/FileManager/GetImage',
                uploadUrl: '/FileManager/FileManager/Upload',
                downloadUrl: '/FileManager/FileManager/Download'
            }" :uploadSettings="{
    allowedExtensions: '.png,.jpg',
    maxFileSize: 20 * 1000 * 1000,
    autoClose: true
}" :allowMultiSelection="false" @fileOpen="chooseFile($event)">
            </FileManagerComponent>
        </Dialog>
    </div>
</template>

<script setup>
import { ref, provide, onMounted, computed } from 'vue';
import { useAuth } from '../../stores/auth'
import { useAxios } from "../../service/axios";
import { FileManagerComponent, DetailsView, NavigationPane, Toolbar } from "@syncfusion/ej2-vue-filemanager";
// import the component
import Treeselect from 'vue3-acies-treeselect'
// import the styles
import 'vue3-acies-treeselect/dist/vue3-treeselect.css'

import "@syncfusion/ej2-base/styles/material.css";
import "@syncfusion/ej2-icons/styles/material.css";
import "@syncfusion/ej2-inputs/styles/material.css";
import "@syncfusion/ej2-popups/styles/material.css";
import "@syncfusion/ej2-buttons/styles/material.css";
import "@syncfusion/ej2-splitbuttons/styles/material.css";
import "@syncfusion/ej2-navigations/styles/material.css";
import "@syncfusion/ej2-layouts/styles/material.css";
import "@syncfusion/ej2-grids/styles/material.css";
import "@syncfusion/ej2-vue-filemanager/styles/material.css";


import AlertError from '../../components/AlertError.vue';
import AlertSuccess from '../../components/AlertSuccess.vue';
import Dialog from 'primevue/dialog';
import { storeToRefs } from 'pinia'
import { useRouter, useRoute } from 'vue-router';
provide('filemanager', [DetailsView, NavigationPane, Toolbar]);
const router = useRouter();
const route = useRoute();
const messageError = ref();
const messageSuccess = ref();
const fileDialog = ref();
const { axiosinstance } = useAxios();
const store = useAuth();
const { roles, departments, data } = storeToRefs(store)
const user = store.user;
// console.log(user);
// const model = ref();
// const dataRoles = ref([]);
onMounted(() => {
    store.fetchRoles();
    store.fetchDepartment();
    store.fetchData(route.params.id);
})

const submit = () => {
    var vaild = $("#form").valid();
    if (!vaild) {
        return;
    }
    let formData = $("#form").serialize()
    axiosinstance.post("/v1/user/Edit", formData).then((response) => {
        return response.data
    }).then((response) => {
        messageError.value = '';
        messageSuccess.value = '';
        if (response.success) {
            messageSuccess.value = response.message;
        } else {
            messageError.value = response.message;
        }
        // console.log(response)
    })
}

const change_pass = () => {
    var vaild = $("#form-pass").valid();
    if (!vaild) {
        return;
    }
    let formData = $("#form-pass").serialize()
    axiosinstance.post("/v1/user/changepassword", formData).then((response) => {
        return response.data
    }).then((response) => {
        messageError.value = '';
        messageSuccess.value = '';
        if (response.success) {
            messageSuccess.value = response.message;
        } else {
            messageError.value = response.message;
        }
        $(".modal-pass").modal("hide");
        // console.log(response)
    })
}

const chooseFile = (e) => {
    var details = e.fileDetails;
    if (details.isFile) {
        var filterPath = details.filterPath.replace(/\\\\/g, "\/");
        var name = details.name;
        var path = "/private/upload/" + user.id  + filterPath + name;
        data.value.image_url = path;
        fileDialog.value = false;
    }
}
</script>