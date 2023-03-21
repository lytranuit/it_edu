<template>
    <div>
        <AlertError :message="messageError" v-if="messageError" />
        <ConfirmDialog></ConfirmDialog>
        <form method="POST" id="form" class="row">
            <div class="col-12">
                <div class="d-inline-block w-100">
                    <span class="page-title">Chỉnh sửa #{{ data.id }}</span>
                    <Button label="Lưu lại" icon="pi pi-save" class="p-button-success p-button-sm float-right"
                        @click="submit()"></Button>
                </div>
            </div>
            <div class="col-12 mt-3">
                <div class="row">
                    <div class="col-md-9">
                        <section class="card card-fluid">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-9 mt-2 title">
                                        <b class="col-form-label">Tiêu đề:<span class="text-danger">*</span></b>
                                        <div class="pt-1">
                                            <input class="form-control form-control-sm" type="text" name="name" required=""
                                                maxlength="250" v-model="data.title">
                                        </div>
                                    </div>
                                    <div class="col-md-3 mt-2">
                                        <b class="col-form-label">Công bố:</b>
                                        <div class="pt-1">
                                            <InputSwitch v-model="data.is_active" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <b class="col-12 col-form-label">Mô tả:</b>
                                    <div class="col-12">
                                        <textarea class="form-control" name="description" rows="10" maxlength="2000"
                                            v-model="data.description"></textarea>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mt-2">
                                        <b class="col-form-label">Thời gian làm bài:<span class="text-danger">*</span></b>
                                        <div class="pt-1">
                                            <div class="p-inputgroup w-full">
                                                <InputNumber v-model="data.duration" />
                                                <span class="p-inputgroup-addon">phút</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6 mt-2">
                                        <b class="col-form-label">Số điểm đạt:<span class="text-danger">*</span></b>
                                        <div class="pt-1">
                                            <div class="p-inputgroup w-full">
                                                <InputNumber v-model="data.point_pass" />
                                                <span class="p-inputgroup-addon">%</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </section>
                        <section class="card card-fluid">
                            <div class="card-header">
                                <b>Câu hỏi</b>
                            </div>
                            <div class="card-body">
                                <div v-for="(item, index) in questions" class="d-flex align-items-center my-2">
                                    <span class="mr-2">{{ index + 1 }}. </span>
                                    <span>Từ Topic</span>
                                    <SelectTopic v-model:value="item.topic_id" style="width: 200px;display: inline-block;"
                                        class="mx-2" @update:value="topic_change(item)" />
                                    lấy
                                    <InputText v-model="item.num_question" style="width: 50px;" class="mx-2" />
                                    /
                                    <InputText v-model="item.num_topic_question" style="width: 50px;" class="mx-2"
                                        disabled="" />
                                    câu hỏi
                                    <button class="p-panel-header-icon p-link ml-3" @click="removeQuestion(item)"
                                        type="button">
                                        <i class="pi pi-times text-danger" role="button"></i>
                                    </button>
                                </div>
                                <Button label="Thêm câu hỏi" icon="pi pi-plus" class="p-button-success p-button-sm mt-3"
                                    @click="addQuestion()"></Button>
                            </div>
                        </section>
                    </div>
                    <div class="col-md-3">
                        <div class="pt-2 pt-md-0">
                            <div class="card border">
                                <div class="card-header">
                                    <strong>Hình đại diện</strong>
                                </div>
                                <div class="card-body text-center">
                                    <FileManager v-model:value="data.image_url"></FileManager>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </form>
        <Loading :waiting="waiting"></Loading>
    </div>
</template>

<script setup>

import SelectTopic from '../../components/Select/SelectTopic.vue'
import AlertError from '../../components/AlertError.vue';
import FileManager from '../../components/Dialog/FileManager.vue';
import Button from 'primevue/button';
import Checkbox from 'primevue/checkbox';
import Dialog from 'primevue/dialog';
import Panel from 'primevue/panel';
import InputSwitch from 'primevue/inputswitch';
import InputNumber from 'primevue/inputnumber';
import InputText from 'primevue/inputtext';
import FileUpload from 'primevue/fileupload';
import Editor from 'primevue/editor';
import Dropdown from 'primevue/dropdown';
import ConfirmDialog from 'primevue/confirmdialog';
import { storeToRefs } from 'pinia'

import { useStore } from "./exam";
import Loading from '../../components/Loading.vue';
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
const route = useRoute();
const store = useStore();
const { waiting, data, messageError, questions } = storeToRefs(store);
const {
    topic_change,
    addQuestion,
    removeQuestion,
    submit,
    getData
} = store;
onMounted(() => {
    getData(route.params.id);
})

</script>

<style scoped></style>