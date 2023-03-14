<template>
    <div>
        <ConfirmDialog></ConfirmDialog>
        <form method="POST" id="form" class="row">
            <div class="col-12">
                <div class="d-inline-block w-100">
                    <span class="page-title">Tạo mới</span>
                    <Button label="Tạo mới" icon="pi pi-plus" class="p-button-success p-button-sm float-right"></Button>
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
                                                placeholder="Tiếng việt" autocomplete="off" maxlength="250"
                                                v-model="data.title">
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
                                    <img :src="data.image_url" class="image_feature"
                                        style="max-height: 100px; max-width: 100%; cursor:;" @click="fileDialog = true">
                                </div>
                            </div>
                        </div>
                        <div class="pt-2 pt-md-0">
                            <div class="card border">
                                <div class="card-header">
                                    <strong>Thời hạn đào tạo</strong>
                                </div>
                                <div class="card-body">
                                    <div class="mt-2">
                                        <b class="col-form-label">Ngày bắt đầu:<span class="text-danger">*</span></b>
                                        <div class="pt-1">
                                            <input id="date_start" class="form-control form-control-sm" type="date"
                                                name="date_start" autocomplete="off" required v-model="data.date_start">
                                        </div>
                                    </div>
                                    <div class="mt-2">
                                        <b class="col-form-label">Ngày hết hạn:<span class="text-danger">*</span></b>
                                        <div class="pt-1">
                                            <input id="date_end" class="form-control form-control-sm " type="date"
                                                name="date_end" autocomplete="off" required v-model="data.date_end">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-12">
                        <h4>
                            Khóa học
                        </h4>
                        <section class="card card-fluid">
                            <div class="card-body">
                                <Panel v-for="chapter in chapterList" class="mt-3">
                                    <template #header>
                                        <input class="form-control form-control-sm mr-3" style="max-width:500px;"
                                            v-model="chapter.title" />
                                    </template>
                                    <template #icons>
                                        <button class="p-panel-header-icon p-link mr-2" @click="removeChapter(chapter)"
                                            type="button">
                                            <i class="pi pi-times text-danger" role="button"></i>
                                        </button>
                                    </template>
                                    <ul class="list-unstyled mb-0">
                                        <li class="d-flex align-items-center" v-for="lesson in chapter.lessons">
                                            <i class="mr-3 fas fa-check-circle" v-if="lesson.type == 1"></i>
                                            <span class="content-title">{{ lesson.title }}</span>
                                            <div>
                                                <button class="p-panel-header-icon p-link mr-2" type="button"><i
                                                        class="pi pi-times text-danger" role="button"></i></button>
                                            </div>
                                        </li>
                                    </ul>
                                    <Button label="Thêm bài học" icon="pi pi-plus" class="p-button-success p-button-sm mt-3"
                                        @click="addLesson(chapter)"></Button>
                                </Panel>

                                <Button label="Thêm chương trình học" icon="pi pi-plus"
                                    class="p-button-success p-button-sm mt-5" @click="addChapter()"></Button>
                            </div>
                        </section>
                    </div>
                </div>

            </div>
        </form>

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
        <Dialog v-model:visible="lessonDialog" header="Bài học" :modal="true" class="p-fluid" :style="{ width: '50vw' }">

            <div class="row mb-2">
                <div class="field col">
                    <label for="name">Tiêu đề:<span class="text-danger">*</span></label>
                    <InputText id="name" class="p-inputtext-sm" v-model="model.title" />
                </div>
                <div class="field col">
                    <label for="name">Loại bài học</label>
                    <Dropdown inputClass="p-inputtext-sm" :options="lessonTypes" optionLabel="label" optionValue="id"
                        v-model="model.type" />

                </div>
            </div>
            <div class="row mb-2">
                <div class="field col-12">
                    <label for="name">Nội dung</label>
                    <Editor v-if="model.type == 1" editorStyle="height: 320px" />
                    <FileUpload v-if="model.type == 2" :multiple="false" :maxFileSize="20 * 1000 * 1000"
                        @select="file($event)" :fileLimit="1" accept="video/*,.pdf,.doc,.docx,.ppt,.pttx"
                        @remove="file($event)">
                        <template #empty>
                            <p>Drag and drop files to here to upload.</p>
                        </template>
                        <template #header="{ chooseCallback, files }">
                            <div class="d-flex d-flex-wrap justify-content-between align-items-center">
                                <Button label="Chọn" icon="pi pi-plus" class="p-button-primary p-button-sm"
                                    @click="chooseCallback()"></Button>
                            </div>
                        </template>
                        <template #content="{ files, removeFileCallback }">
                            <div v-if="files.length > 0">
                                <div class="p-fileupload-file" v-for="(file, index) of files"
                                    :key="file.name + file.type + file.size">
                                    <img role="presentation" :alt="file.name" :src="check_presentation(file)" width="50">
                                    <div class="p-fileupload-file-details">
                                        <div class="p-fileupload-file-name">{{ file.name }}</div>
                                        <span class="p-fileupload-file-size">{{ formatSize(file.size) }}</span>
                                        <span
                                            class="p-badge p-component p-badge-warning p-fileupload-file-badge">Pending</span>
                                    </div>
                                    <div class="p-fileupload-file-actions">
                                        <button class="p-panel-header-icon p-link mr-2" @click="removeFileCallback(index)"
                                            type="button">
                                            <i class="pi pi-times text-danger" role="button"></i>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </template>
                    </FileUpload>
                </div>
                <div class="field col-12 mt-3">
                    <label for="name">Tài liệu đính kèm</label>
                    <FileUpload :multiple="true" :maxFileSize="20 * 1000 * 1000" @select="attachments($event)"
                        @remove="attachments($event)">
                        <template #empty>
                            <p>Drag and drop files to here to upload.</p>
                        </template>
                        <template #header="{ chooseCallback, files }">
                            <div class="d-flex d-flex-wrap justify-content-between align-items-center">
                                <Button label="Chọn" icon="pi pi-plus" class="p-button-primary p-button-sm"
                                    @click="chooseCallback()"></Button>
                            </div>
                        </template>
                        <template #content="{ files, removeFileCallback }">
                            <div v-if="files.length > 0">
                                <div class="p-fileupload-file" v-for="(file, index) of files"
                                    :key="file.name + file.type + file.size">
                                    <img role="presentation" :alt="file.name" :src="check_presentation(file)" width="50">
                                    <div class="p-fileupload-file-details">
                                        <div class="p-fileupload-file-name">{{ file.name }}</div>
                                        <span class="p-fileupload-file-size">{{ formatSize(file.size) }}</span>
                                        <span
                                            class="p-badge p-component p-badge-warning p-fileupload-file-badge">Pending</span>
                                    </div>
                                    <div class="p-fileupload-file-actions">
                                        <button class="p-panel-header-icon p-link mr-2" @click="removeFileCallback(index)"
                                            type="button">
                                            <i class="pi pi-times text-danger" role="button"></i>
                                        </button>
                                    </div>
                                </div>


                            </div>
                        </template>
                    </FileUpload>
                </div>
            </div>
            <template #footer>
                <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="lessonDialog = false"></Button>
                <Button label="Save" icon="pi pi-check" class="p-button-text" @click="saveLesson()"></Button>
            </template>
        </Dialog>

    </div>
</template>

<script setup>
import { ref, provide, onMounted, computed } from 'vue';
import { FileManagerComponent, DetailsView, NavigationPane, Toolbar } from "@syncfusion/ej2-vue-filemanager";
import { useAuth } from '../../stores/auth'
import { useAxios } from "../../service/axios";
// import the styles


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



// import AlertError from '../../components/AlertError.vue';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import Panel from 'primevue/panel';
import InputSwitch from 'primevue/inputswitch';
import InputText from 'primevue/inputtext';
import ProgressBar from 'primevue/progressbar';
import FileUpload from 'primevue/fileupload';
import Editor from 'primevue/editor';
import Dropdown from 'primevue/dropdown';
import ConfirmDialog from 'primevue/confirmdialog';
import { useConfirm } from "primevue/useconfirm";
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router';
import { rand } from '../../utilities/rand';
provide('filemanager', [DetailsView, NavigationPane, Toolbar]);

const confirm = useConfirm();
const router = useRouter();
const messageError = ref();
const messageSuccess = ref();
const lessonTypes = ref([
    { id: 1, label: "Văn bản" },
    { id: 2, label: "Word/PDF/PowerPoint/Video" },
    { id: 3, label: "Bài kiểm tra" }
])

const fileDialog = ref();
const lessonDialog = ref();

const model = ref();

const { axiosinstance } = useAxios();
const store = useAuth();
const user = store.user;
const data = ref({
    tmp: rand(),
    image_url: '/private/upload/5a375cd2-1908-4784-9b7b-d470e2d63376/default/p-2.svg'
});
const chapterList = ref([]);
onMounted(() => {

})
const submit = () => {
    var vaild = $("#form").valid();
    if (!vaild) {
        return;
    }
    let formData = $("#form").serialize()
    axiosinstance.post("/v1/user/Create", formData).then((response) => {
        return response.data
    }).then((response) => {
        messageError.value = '';
        if (response.success) {
            router.push("/user")
        } else {
            messageError.value = response.message;
        }
    })
}

const chooseFile = (e) => {
    var details = e.fileDetails;
    if (details.isFile) {
        var filterPath = details.filterPath.replace(/\\\\/g, "\/").replace(/\\/g, "\/");
        var name = details.name;
        var path = "/private/upload/" + user.id + filterPath + name;
        data.value.image_url = path;
        fileDialog.value = false;
    }
}
const addChapter = () => {
    chapterList.value.push({ title: "Chapter", lessons: [] });
}
const removeChapter = (chapter) => {
    confirm.require({
        message: 'Bạn có muốn xóa Chương trình học này?',
        header: 'Xác nhận xóa',
        icon: 'pi pi-info-circle',
        acceptClass: 'p-button-danger',
        accept: () => {
            chapterList.value = chapterList.value.filter(item => item != chapter);
        },
        reject: () => {
        }
    });

}
const addLesson = (chapter) => {
    lessonDialog.value = true;
    model.value = {
        type: 1,
        chapter: chapter,
    }
    // chapter.lessons.push({ title: "Lesson" });
}
const saveLesson = () => {
    if (!model.value.title || model.value.title.trim() == '') {
        alert("Chưa nhập tiêu đề!");
        return false;
    }
    var chapter = model.value.chapter;
    delete model.value.chapter;
    chapter.lessons.push(model.value);

    lessonDialog.value = false;

}
const attachments = (e) => {
    console.log(e.files);
    model.value.attachments = e.files;
}
const file = (e) => {
    model.value.file = e.files.length > 0 ? e.files[0] : null;
}
const check_presentation = (file) => {
    var image = file.objectURL;
    var type = file.type;
    var list = file.name.split(".");
    var ext = list.pop();
    console.log(type);
    console.log(type.indexOf("image"));
    if (type.indexOf("image") == -1) {
        image = "/images/file.png"
    }
    return image;
}
const formatSize = (bytes) => {
    if (bytes === 0) {
        return '0 B';
    }

    let k = 1000,
        dm = 3,
        sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
        i = Math.floor(Math.log(bytes) / Math.log(k));

    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
}
</script>

<style scoped></style>