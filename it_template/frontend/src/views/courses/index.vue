<template>
    <div class="layout-wrapper">
        <Toast />
        <Topbar></Topbar>
        <div class="layout-main-container">
            <div class="layout-main" :class="{ 'open-lecture-list': visibleLecture }">
                <div class="study-content">
                    <div class="section-main-content bg-dark">
                        <div class="container">
                            <div class="playing-lecture d-flex justify-content-center w-100" style="padding: 40px 0px;">
                                <embed src="/1677749324.pdf"
                                    style="min-height: 450px;height: calc(100vh - 270px); width: 100%; border: 1px solid white;"
                                    type="application/pdf">
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
                                <Accordion class="accordion-custom" :multiple="true" :activeIndex="[0]">
                                    <AccordionTab v-for="option in chapterList" :header="option.title">
                                        <div class="list-section-body">
                                            <ul class="list-unstyled mb-0">
                                                <li v-for="lesson of option.lessons"
                                                    class="d-flex align-items-center pointer">
                                                    <i class="mr-3" :class="icon_class_lesson(lesson.id)"></i>
                                                    <span class="content-title">
                                                        {{ lesson.title }}
                                                    </span>
                                                    <span class="duration"></span>
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
import { onMounted, ref, watch, computed } from 'vue';
import { useAxios } from "../../service/axios";
import Topbar from '../../components/courses/Topbar.vue';
// import the component
import Accordion from 'primevue/accordion';
import AccordionTab from 'primevue/accordiontab';

import Toast from 'primevue/toast';
import { useToast } from "primevue/usetoast";
import { storeToRefs } from "pinia";
import { useCoursesStore } from '../../stores/courses';
const courses_store = useCoursesStore();
const toast = useToast();

const { axiosinstance } = useAxios();

const { chapterList, totalLesson, personInfo } = storeToRefs(courses_store);

const visibleLecture = ref(true);
const currentLesson = ref();

const toogle_lecture = () => {
    visibleLecture.value = !visibleLecture.value;
}
const icon_class_lesson = (lesson_id) => {
    let list_lesson_pass = personInfo.value.list_lesson_pass;
    let current_lesson = personInfo.value.current_lesson;
    if (list_lesson_pass.indexOf(lesson_id) != -1) {
        return 'fas fa-check-circle text-success';
    } else if (current_lesson == lesson_id) {
        return 'far fa-play-circle text-danger';
    }
    return 'far fa-circle';
}
onMounted(() => {

})
</script>
<style scoped lang="scss">
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

.list-lecture .lecture-body .list-section .list-section-body ul li .content-title {
    width: calc(100% - 80px)
}

.list-lecture .lecture-body .list-section .list-section-body ul li .duration {
    width: 60px;
    text-align: end
}
</style>