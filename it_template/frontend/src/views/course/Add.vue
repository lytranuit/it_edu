<template>
    <div>
        <AlertError :message="messageError" v-if="messageError" />
        <ConfirmDialog></ConfirmDialog>
        <form method="POST" id="form" class="row" enctype="multipart/form-data" action="/v1/course/Create">
            <div class="col-12">
                <div class="d-inline-block w-100">
                    <span class="page-title">Tạo mới</span>
                    <Button label="Tạo mới" icon="pi pi-plus" class="p-button-success p-button-sm float-right"
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
                                    <FileManager v-model:value="data.image_url"></FileManager>
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
                                        <li class="row align-items-center" v-for="lesson in chapter.lessons"
                                            style="padding: 10px;border-bottom: 1px dotted;">
                                            <div class="col-9">
                                                <i class="mr-2 fas fa-font" v-if="lesson.type == 1"></i>
                                                <i class="mr-2 fas fa-video" v-else-if="lesson.type == 2"></i>
                                                <i class="mr-2 fab fa-youtube" v-else-if="lesson.type == 3"></i>
                                                <i class="mr-2 fas fa-file" v-else-if="lesson.type == 4"></i>
                                                <i class="mr-2 fas fa-question" v-else-if="lesson.type == 5"></i>
                                                <span class="content-title">{{ lesson.title }}</span>
                                            </div>
                                            <div class="col-3 text-right">
                                                <div class="duration mr-3 d-inline-block" v-if="lesson.duration > 0">{{
                                                    formatTime(lesson.duration) }}</div>
                                                <button class="p-panel-header-icon p-link mr-2 text-warning" type="button"
                                                    @click="editLesson(lesson, chapter)"><i
                                                        class="fas fa-edit"></i></button>
                                                <button class="p-panel-header-icon p-link mr-2" type="button"
                                                    @click="removeLesson(lesson, chapter)"><i
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

        <Dialog v-model:visible="lessonDialog" header="Bài học" :modal="true" class="p-fluid" :style="{ width: '50vw' }">

            <div class="row mb-2">
                <div class="field col">
                    <label for="name">Tiêu đề:<span class="text-danger">*</span></label>
                    <InputText id="name" class="p-inputtext-sm" v-model="model.title" placeholder="Tiêu đề" />
                </div>
                <div class="field col">
                    <label for="name">Loại bài học</label>
                    <Dropdown inputClass="p-inputtext-sm" :options="lessonTypes" optionLabel="label" optionValue="id"
                        v-model="model.type" />

                </div>
            </div>
            <div class="row mb-2">
                <div class="field col-12" v-if="model.type == 1">
                    <label for="name">Nội dung</label>
                    <Editor editorStyle="height: 320px" v-model="model.text" />
                </div>
                <div class="field col-12" v-else-if="model.type == 2">
                    <label for="name">Nội dung</label>
                    <FileUpload :multiple="false" :maxFileSize="20 * 1000 * 1000" @select="file($event)" :fileLimit="1"
                        accept="video/*" @remove="file($event)">
                        <template #empty>
                            <p class="text-center">
                                <b>Upload video bài học</b>
                            <div>(Các định dạng file hỗ trợ .mp4)</div>
                            </p>
                        </template>
                        <template #header="{ chooseCallback, files }">
                            <div class="d-flex d-flex-wrap justify-content-between align-items-center">
                                <Button label="Chọn" icon="pi pi-plus" class="p-button-primary p-button-sm"
                                    @click="chooseCallback()"></Button>
                            </div>
                        </template>
                        <template #content="{ files, removeFileCallback }">
                            <div v-if="model.file">
                                <div class="p-fileupload-file" :key="model.file.name + model.file.type + model.file.size">
                                    <img role="presentation" :alt="model.file.name" :src="check_presentation(model.file)"
                                        width="50">
                                    <div class="p-fileupload-file-details">
                                        <div class="p-fileupload-file-name">{{ model.file.name }}</div>
                                        <span class="p-fileupload-file-size">{{ formatSize(model.file.size) }}</span>
                                        <span
                                            class="p-badge p-component p-badge-warning p-fileupload-file-badge">Pending</span>
                                    </div>
                                    <div class="p-fileupload-file-actions">
                                        <button class="p-panel-header-icon p-link mr-2" @click="removeFileCallback(0)"
                                            type="button">
                                            <i class="pi pi-times text-danger" role="button"></i>
                                        </button>
                                    </div>
                                </div>
                            </div>

                            <div v-if="model.file_up">
                                <div class="p-fileupload-file"
                                    :key="model.file_up.name + model.file_up.mineType + model.file_up.size">
                                    <img role="presentation" :alt="model.file_up.name"
                                        :src="check_presentation_file_uploaded(model.file_up)" width="50">
                                    <div class="p-fileupload-file-details">
                                        <div class="p-fileupload-file-name">{{ model.file_up.name }}</div>
                                        <span class="p-fileupload-file-size">{{ formatSize(model.file_up.size) }}</span>
                                    </div>
                                    <div class="p-fileupload-file-actions">
                                    </div>
                                </div>
                            </div>
                        </template>
                    </FileUpload>
                </div>
                <template v-else-if="model.type == 3">
                    <div class="field col-12 text-center my-2">
                        Thêm bài học từ Youtube
                        <div>(Ví dụ url: https://www.youtube.com/watch?v=qhSds5jPnFo)</div>
                    </div>
                    <div class="field col-6 text-center">
                        <InputText class="p-inputtext-sm" v-model="model.file_url" placeholder="Nhập URL"
                            @change="getInfoYoutube($event)" />
                    </div>
                    <div class="field col-6 text-center">
                        <InputText class="p-inputtext-sm" v-model="model.duration" placeholder="Thời lượng video" />
                    </div>

                </template>


                <div class="field col-12" v-else-if="model.type == 4">
                    <label for="name">Nội dung</label>

                    <FileUpload :multiple="false" :maxFileSize="20 * 1000 * 1000" @select="file($event)" :fileLimit="1"
                        accept=".pdf" @remove="file($event)">
                        <template #empty>
                            <p class="text-center">
                                <b>Upload video bài học</b>
                            <div>(Các định dạng file hỗ trợ .pdf)</div>
                            </p>
                        </template>
                        <template #header="{ chooseCallback, files }">
                            <div class="d-flex d-flex-wrap justify-content-between align-items-center">
                                <Button label="Chọn" icon="pi pi-plus" class="p-button-primary p-button-sm"
                                    @click="chooseCallback()"></Button>
                            </div>
                        </template>
                        <template #content="{ files, removeFileCallback }">
                            <div v-if="model.file">
                                <div class="p-fileupload-file" :key="model.file.name + model.file.type + model.file.size">
                                    <img role="presentation" :alt="model.file.name" :src="check_presentation(model.file)"
                                        width="50">
                                    <div class="p-fileupload-file-details">
                                        <div class="p-fileupload-file-name">{{ model.file.name }}</div>
                                        <span class="p-fileupload-file-size">{{ formatSize(model.file.size) }}</span>
                                        <span
                                            class="p-badge p-component p-badge-warning p-fileupload-file-badge">Pending</span>
                                    </div>
                                    <div class="p-fileupload-file-actions">
                                        <button class="p-panel-header-icon p-link mr-2" @click="removeFileCallback(0)"
                                            type="button">
                                            <i class="pi pi-times text-danger" role="button"></i>
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <div v-if="model.file_up">
                                <div class="p-fileupload-file"
                                    :key="model.file_up.name + model.file_up.mineType + model.file_up.size">
                                    <img role="presentation" :alt="model.file_up.name"
                                        :src="check_presentation_file_uploaded(model.file_up)" width="50">
                                    <div class="p-fileupload-file-details">
                                        <div class="p-fileupload-file-name">{{ model.file_up.name }}</div>
                                        <span class="p-fileupload-file-size">{{ formatSize(model.file_up.size) }}</span>
                                    </div>
                                    <div class="p-fileupload-file-actions">
                                    </div>
                                </div>
                            </div>
                        </template>
                    </FileUpload>
                </div>

                <div class="field col-12" v-if="model.type == 5">
                    <label>Bài kiểm tra</label>
                    <SelectExam v-model:value="model.exam_id"></SelectExam>
                </div>

                <div class="field col-12 mt-3">
                    <label for="name">Tài liệu đính kèm</label>
                    <FileUpload :multiple="true" :maxFileSize="20 * 1000 * 1000" @select="attachments($event)"
                        @remove="attachments($event)">
                        <template #empty>
                            <p class="text-center">Kéo thả tài liệu vào đây để tải lên.</p>
                        </template>
                        <template #header="{ chooseCallback, files }">
                            <div class="d-flex d-flex-wrap justify-content-between align-items-center">
                                <Button label="Chọn" icon="pi pi-plus" class="p-button-primary p-button-sm"
                                    @click="chooseCallback()"></Button>
                            </div>
                        </template>
                        <template #content="{ files, removeFileCallback }">
                            <div v-if="model.attachments && model.attachments.length > 0">
                                <div class="p-fileupload-file" v-for="(file, index) of model.attachments"
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
                            <div v-if="model.attachments_up && model.attachments_up.length > 0" class="mt-2">
                                <div class="p-fileupload-file" v-for="(file, index) of model.attachments_up"
                                    :key="file.name + file.mineType + file.size">
                                    <img role="presentation" :alt="file.name" :src="check_presentation_file_uploaded(file)"
                                        width="50">
                                    <div class="p-fileupload-file-details">
                                        <div class="p-fileupload-file-name">{{ file.name }}</div>
                                        <span class="p-fileupload-file-size">{{ formatSize(file.size) }}</span>
                                    </div>
                                    <div class="p-fileupload-file-actions">
                                        <button class="p-panel-header-icon p-link mr-2" @click="removeFileAttachment(file)"
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

        <Loading :waiting="waiting"></Loading>
    </div>
</template>

<script setup>

import AlertError from '../../components/AlertError.vue';
import FileManager from '../../components/Dialog/FileManager.vue';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import Panel from 'primevue/panel';
import InputSwitch from 'primevue/inputswitch';
import InputText from 'primevue/inputtext';
import FileUpload from 'primevue/fileupload';
import Editor from 'primevue/editor';
import Dropdown from 'primevue/dropdown';
import ConfirmDialog from 'primevue/confirmdialog';
import { storeToRefs } from 'pinia'

import { useCoursesStore } from "./courses";
import Loading from '../../components/Loading.vue';
import { onMounted } from 'vue';
import SelectExam from '../../components/Select/SelectExam.vue';
import { formatSize, formatTime } from '../../utilities/util'
const courses_store = useCoursesStore();
const { chapterList, data, lessonTypes, model, lessonDialog, messageError, waiting } = storeToRefs(courses_store);
const { submit,
    addChapter,
    removeChapter,
    addLesson,
    editLesson,
    removeLesson,
    saveLesson,
    attachments,
    file,
    check_presentation,
    check_presentation_file_uploaded,
    removeFileAttachment,
    getInfoYoutube
} = courses_store;

onMounted(() => {
    reset();
})


</script>

<style scoped></style>