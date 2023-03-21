<template>
    <div>
        <AlertError :message="messageError" v-if="messageError" />
        <ConfirmDialog></ConfirmDialog>
        <form method="POST" id="form" class="row" enctype="multipart/form-data" action="/v1/course/Create">
            <div class="col-12">
                <div class="d-inline-block w-100">
                    <span class="page-title">Chỉnh sửa #{{ data.id }}</span>
                    <Button label="Lưu lại" icon="pi pi-save" class="p-button-success p-button-sm float-right"
                        @click="submit()"></Button>
                </div>
            </div>
            <div class="col-12 mt-3">
                <div class="row">
                    <div class="col-md-3">
                        <section class="card card-fluid">
                            <div class="card-header">
                                Cài đặt câu hỏi
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-12 mt-2">
                                        <b class="col-form-label">Điểm cho câu hỏi: <span class="text-danger">*</span></b>
                                        <div class="pt-1">
                                            <input class="form-control form-control-sm" type="number" v-model="data.point">
                                        </div>
                                    </div>
                                    <div class="col-md-12 mt-2">
                                        <b class="col-form-label">Topic cho câu hỏi: <span class="text-danger">*</span></b>
                                        <div class="pt-1">
                                            <SelectTopic v-model:value="data.topic_id"></SelectTopic>
                                        </div>
                                    </div>
                                    <div class="col-md-12 mt-2">
                                        <b class="col-form-label">Level cho câu hỏi: <span class="text-danger">*</span></b>
                                        <div class="pt-1">
                                            <select class="form-control form-control-sm" placeholder="Level"
                                                v-model="data.level">
                                                <option value="1">Dễ</option>
                                                <option value="2" selected>Bình thường</option>
                                                <option value="3">Khó</option>
                                                <option value="3">Cực khó</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-12 mt-2">
                                        <b class="col-form-label">Đảo câu trả lời:</b>
                                        <div class="pt-1">
                                            <InputSwitch v-model="data.is_random" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </section>
                    </div>
                    <div class="col-md-9">
                        <section class="card card-fluid">
                            <div class="card-header">
                                Nhập câu hỏi
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-12 mt-2">
                                        <Editor v-model="data.question" editorStyle="height: 50px" />
                                    </div>
                                </div>
                            </div>
                        </section>
                        <section class="card card-fluid mt-2">
                            <div class="card-header">
                                Nhập câu trả lời
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-12 mt-2" v-for="(anwser, index) in anwsers">
                                        <div class="d-flex align-items-center mb-2">
                                            <span>{{ index + 1 }}.</span>
                                            <div class="d-flex align-items-center ml-1">
                                                <Checkbox v-model="anwser.is_true" :inputId="anwser.id" name="is_true"
                                                    :value="anwser.id" />
                                                <label :for="anwser.id" style="margin-bottom: 0; margin-left: 5px;">Đây là
                                                    đáp án</label>
                                            </div>
                                            <button class="p-panel-header-icon p-link ml-auto" type="button"
                                                @click="removeAnwser(anwser)"><i class="pi pi-times text-danger"
                                                    role="button"></i></button>

                                        </div>
                                        <Editor v-model="anwser.anwser" editorStyle="height: 50px" />
                                    </div>
                                    <Button label="Thêm câu trả lời" icon="pi pi-plus"
                                        class="p-button-success p-button-sm mt-3" @click="addAnwser()"></Button>

                                </div>
                            </div>
                        </section>

                        <section class="card card-fluid mt-2">
                            <div class="card-header">
                                Giải thích đáp án
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-12 mt-2">
                                        <Editor v-model="data.description" editorStyle="height: 50px" />
                                    </div>
                                </div>
                            </div>
                        </section>
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
import InputText from 'primevue/inputtext';
import FileUpload from 'primevue/fileupload';
import Editor from 'primevue/editor';
import Dropdown from 'primevue/dropdown';
import ConfirmDialog from 'primevue/confirmdialog';
import { storeToRefs } from 'pinia'

import { useQuestionStore } from "./question";
import Loading from '../../components/Loading.vue';
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
const route = useRoute();
const store = useQuestionStore();
const { waiting, data, anwsers, messageError } = storeToRefs(store);
const {
    addAnwser,
    removeAnwser,
    submit
} = store;


onMounted(() => {
    store.getData(route.params.id);
})
</script>

<style scoped></style>