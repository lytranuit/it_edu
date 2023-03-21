<template>
    <div class="layout-wrapper">
        <Toast />
        <ConfirmDialog></ConfirmDialog>
        <Topbar></Topbar>
        <Loading :waiting="waiting"></Loading>
        <div class="layout-main-container">
            <div class="layout-main" :class="{ 'open-lecture-list': visibleLecture }">
                <div class="study-content">
                    <div class="section-main-content bg-dark">
                        <div class="container">
                            <div class="playing-lecture d-flex justify-content-center w-100"
                                style="padding: 40px 0px;min-height: 80vh;">

                                <div style="background-color: white;padding: 20px;width: 100%;"
                                    v-if="currentLesson.type == 1" v-html="currentLesson.text"></div>
                                <embed :src="currentLesson.file_up.url"
                                    style="height: 70vh; width: 100%; border: 1px solid white;" type="application/pdf"
                                    v-else-if="currentLesson.type == 4">
                                <iframe style="height: 70vh; width: 100%; border: 1px solid white;"
                                    :src="'https://www.youtube.com/embed/' + currentLesson.youtube_id"
                                    v-else-if="currentLesson.type == 3">
                                </iframe>
                                <video style="height: 70vh; width: 100%; border: 1px solid white;" autoplay controls
                                    v-else-if="currentLesson.type == 2">
                                    <source :src="currentLesson.file_up.url" type="video/mp4">
                                    Your browser does not support the video tag.
                                </video>
                            </div>
                        </div>
                    </div>
                    <div class="container">
                        <div style="max-width: 50rem;width: 100%" class="mx-auto">
                            <div class="file-page-playing my-4"
                                v-if="currentLesson.attachments_up && currentLesson.attachments_up.length">
                                <div class="file-page-playing-list">
                                    <div class="row">
                                        <div class="col-12">
                                            <p class="font-weight-bolder color_label f-18 mb-0">Tài liệu đính kèm bài giảng
                                            </p>
                                        </div>
                                        <div class="col-12" v-for="attachment in currentLesson.attachments_up">
                                            <div class="list-attachment mt-3 d-flex justify-content-between">
                                                <i class="fas fa-file" style="font-size:40px;"></i>
                                                <div class="file-info my-auto">
                                                    <p class="file-name mb-0 ellipsis line-clamp"
                                                        style="overflow-wrap: anywhere;">
                                                        {{ attachment.name }}
                                                    </p>
                                                    <span>
                                                        {{ attachment.ext }}
                                                        <span class="mx-2">|</span>
                                                        {{ formatSize(attachment.size) }}
                                                    </span>
                                                </div>

                                                <div class="my-auto ml-3">
                                                    <a class="btn btn-sm btn-download-file-white" :href="attachment.url"
                                                        :download="attachment.name" style="text-decoration: none">
                                                        <i class="fas fa-download"></i>
                                                        Tải về
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <div class="toogle-lecture-list-btn" title="Hiển thị danh sách bài học" @click="toogle_lecture()">
                        <div class="d-flex align-items-center">
                            <i class="fas fa-play-circle" style="font-size: 20px;"></i>
                            <div class="content-head ml-2" style="white-space:nowrap">
                                <p class="mb-0">
                                    <strong>Nội dung khóa học</strong><br>
                                </p>
                            </div>
                        </div>
                    </div>
                    <div id="course-lecture-js" class="course-content sticky" style="top: calc(5rem + 2px)">
                        <div class="list-lecture show">
                            <div class="container-list-lecture p-0">
                                <div class="head-list-lecture d-flex justify-content-between">
                                    <div class="d-flex">
                                        <i class="fab fa-discourse align-self-center"></i>
                                        <div class="content-head ml-3" style="white-space:nowrap">
                                            <p class="mb-0">
                                                <strong>Nội dung khóa học</strong><br>
                                                <span>{{ chapterList.length }} Chương, {{ totalLesson }} Bài học</span>
                                            </p>
                                        </div>
                                    </div>
                                    <button class="btn btn-default d-md-block d-none" @click="toogle_lecture()"
                                        style="outline: none;box-shadow: none">
                                        <i class="fas fa-times"></i>
                                    </button>
                                </div>
                                <Accordion class="accordion-custom" :multiple="true" :activeIndex="activeIndex">
                                    <AccordionTab v-for="option in chapterList" :header="option.title"
                                        contentClass="accordion-custom">
                                        <div class="list-section-body">
                                            <ul class="list-unstyled mb-0">
                                                <li class="d-flex align-items-center pointer"
                                                    v-for="lesson of option.lessons"
                                                    :class="{ 'active': currentLesson.id == lesson.id }"
                                                    @click="clickLesson(lesson)">
                                                    <i class="mr-3" :class="icon_class_lesson(lesson)"></i>
                                                    <span class="content-title">
                                                        {{ lesson.title }}
                                                    </span>
                                                    <span class="duration" v-if="lesson.duration > 0">{{
                                                        formatTime(lesson.duration) }}</span>
                                                </li>
                                            </ul>
                                        </div>
                                    </AccordionTab>
                                </Accordion>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { onMounted, watch, } from 'vue';
import Topbar from '../../components/courses/Topbar.vue';
// import the component
import Accordion from 'primevue/accordion';
import AccordionTab from 'primevue/accordiontab';

import Toast from 'primevue/toast';
import { storeToRefs } from "pinia";
import { useCoursesStore1 } from '../../stores/courses';
import { useRoute } from 'vue-router';

import { formatSize, formatTime } from '../../utilities/util'
import Loading from '../../components/Loading.vue';
import ConfirmDialog from 'primevue/confirmdialog';
const route = useRoute();
const courses_store = useCoursesStore1();

const { chapterList, totalLesson, personInfo, currentLesson, activeIndex, course, lessonList, waiting, visibleLecture } = storeToRefs(courses_store);

// console.log(currentLesson.value)
const { toogle_lecture, setCurrentLesson, getData, icon_class_lesson, clickLesson } = courses_store

onMounted(() => {
    getData(route.params.id).then(() => {
        if (route.query.lesson_id > 0)
            setCurrentLesson(route.query.lesson_id);
        else
            setCurrentLesson(lessonList.value[0].id);
    });

})
watch(() => route.query.lesson_id,
    async newId => {
        setCurrentLesson(newId);
    })
</script>
<style scoped lang="scss">
.p-toggleable-content {
    margin: 20px;
}

.toogle-lecture-list-btn {
    position: absolute;
    right: 0;
    top: 24px;
    background: #f7f7f7;
    box-shadow: 0 1px 3px rgb(0 0 0 / 20%);
    border-radius: 15px 0 0 15px;
    padding: 15px 12px;
    cursor: pointer;
    transition: all .3s;
    -webkit-transition: all .3s;
}

.open-lecture-list {
    .course-content {
        transform: translateX(0);
    }

    .study-content {
        width: calc(100% - 405px);
    }
}

.study-content {
    width: 100%;
    transition: all .3s;
    -webkit-transition: all .3s;
}

.course-content {
    width: 400px;
    position: fixed;
    right: 5px;
    top: 55px;
    z-index: 1000;
    background-color: #fff;
    height: 100%;
    transition: transform .3s;
    -webkit-transition: transform .3s;
    transform: translateX(102%);
}

.pointer {
    cursor: pointer;
}

.list-section-body {
    li {
        padding: 0.8rem 0rem;
        color: #334d6e;
        font-size: 12px;
        line-height: 16px;
        padding: 15px;
        border-bottom: 1px dotted black;

        &.active {
            background: #ffe6ee
        }

        &:hover {
            background: #ffe6ee
        }
    }
}

.content-title {
    width: calc(100% - 80px);
}

.layout-main-container {
    padding: 5rem 0rem 0rem 0rem;
}

.layout-main {
    position: relative;
}

.list-lecture {
    width: 400px;
    background: #f4f4f4;
    border-left: 1px solid silver;
    position: absolute;
    top: 0;
    height: 100%;
    right: 0;
    transition: all .3s;
    -webkit-transition: all .3s;
    z-index: 2
}

.list-lecture .container-list-lecture {
    height: 100%
}

.list-lecture .head-list-lecture {
    color: #334d6e;
    padding: .8rem 1.5rem;
    box-shadow: 0 2px 5px rgba(0, 0, 0, .2);
    position: relative;
    background: #f9f9f9
}

.list-lecture .head-list-lecture .content-head {
    line-height: 18px
}

.list-lecture .head-list-lecture .content-head strong {
    font-size: 16px
}

.list-lecture .head-list-lecture .content-head span {
    font-size: 12px
}

.list-lecture .lecture-body {
    height: calc(100% - 60px);
    overflow-y: auto;
    border-bottom: 1px solid #bebebe;
    scrollbar-color: #334d6e #f5f5f5
}

.list-lecture .lecture-body::-webkit-scrollbar-track {
    -webkit-box-shadow: inset 0 0 6px rgba(0, 0, 0, .1);
    background-color: #f5f5f5;
    border-radius: 3px
}

.list-lecture .lecture-body::-webkit-scrollbar {
    width: 8px;
    background-color: #f5f5f5
}

.list-lecture .lecture-body::-webkit-scrollbar-thumb {
    border-radius: 3px;
    -webkit-box-shadow: inset 0 0 6px rgba(0, 0, 0, .1);
    background-image: linear-gradient(120deg, #334d6e, #334d6e)
}

.list-lecture .lecture-body .list-section .list-section-header {
    display: flex;
    padding: .8rem 1.5rem;
    background: #f4f4f4;
    border: 1px solid #bebebe;
    border-left: none;
    justify-content: space-between
}

.list-lecture .lecture-body .list-section .list-section-header .content-head {
    width: calc(100% - 40px)
}

.list-lecture .lecture-body .list-section .list-section-header .content-head .section-title {
    font-size: 14px;
    font-weight: 600;
    color: #334d6e;
    line-height: 18px
}

.list-lecture .lecture-body .list-section .list-section-header .content-head .section-info {
    font-size: 12px;
    color: #334d6e;
    line-height: 16px
}

.list-lecture .lecture-body .list-section .list-section-header button {
    outline: none;
    box-shadow: none;
    font-size: 16px
}

.list-lecture .lecture-body .list-section .list-section-header button:active,
.list-lecture .lecture-body .list-section .list-section-header button:focus,
.list-lecture .lecture-body .list-section .list-section-header button:hover {
    box-shadow: none;
    outline: none
}

.list-lecture .lecture-body .list-section .list-section-body ul li {
    padding: .8rem 1.5rem;
    color: #334d6e;
    font-size: 12px;
    line-height: 16px
}

.list-lecture .lecture-body .list-section .list-section-body ul li.active,
.list-lecture .lecture-body .list-section .list-section-body ul li:hover {
    background: #ffe6ee
}

.content-title {
    width: calc(100% - 80px)
}

.duration {
    width: 60px;
    text-align: end;
    margin-left: auto
}

.file-page-playing-list .list-attachment {
    border: 1px solid #c4c4c4;
    background: #fff;
    border-radius: 4px;
    padding: .5rem
}

.file-page-playing-list .list-attachment .file-info {
    width: calc(100% - 150px)
}

.file-page-playing-list .list-attachment .file-info .file-name {
    font-size: 14px;
    font-weight: 700;
    line-height: 21px;
    color: #334d6e;
    overflow: hidden;
    text-overflow: ellipsis;
    -webkit-line-clamp: 1;
    display: -webkit-box;
    -webkit-box-orient: vertical
}

.file-page-playing-list .list-attachment .file-info span {
    font-size: 12px;
    line-height: 21px;
    color: #6b7077
}

.file-page-playing-list .list-attachment .btn-download-file-white {
    color: #334d6e;
    background: #fff;
    border: 1px solid #334d6e;
    border-radius: 4px;
    font-size: 14px;
    font-weight: 600;
    transition: .2s ease-in-out
}

.file-page-playing-list .list-attachment .btn-download-file-white:active,
.file-page-playing-list .list-attachment .btn-download-file-white:hover {
    background: #e0eeff
}
</style>