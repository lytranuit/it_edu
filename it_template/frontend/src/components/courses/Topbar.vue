<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';

import { storeToRefs } from "pinia";
import { useCoursesStore } from '../../stores/courses';
const courses_store = useCoursesStore();

const { percentageCompleted, courses } = storeToRefs(courses_store);
onMounted(() => {

});
</script>

<template>
    <div class="layout-topbar">
        <router-link to="/" class="layout-topbar-logo justify-content-center">
            <span>
                <img src="../../assets/images/PMP_Stada_Group.png" alt="logo-large" class="logo-lg logo-light">
            </span>
        </router-link>
        <div class="border-right mx-3" style="height: 30px"></div>
        <div class="d-flex mr-3" style="flex:1">
            <div style="max-width: 100%;">
                <h5 class="nav-item mb-0 ellipsis line-clamp">
                    <strong>{{ courses.code }} - {{ courses.title }} </strong>
                </h5>
            </div>

        </div>
        <div>
            <div class="d-flex">
                <div class="progress-study" :data-percentage="percentageCompleted">
                    <span class="progress-left">
                        <span class="progress-bar"></span>
                    </span>
                    <span class="progress-right">
                        <span class="progress-bar"></span>
                    </span>
                    <div class="progress-value">
                        <div class="text-success">
                            {{ percentageCompleted }} %
                        </div>
                    </div>
                </div>
                <p class="ml-2 my-auto d-none d-lg-inline-block">Hoàn thành</p>
            </div>

        </div>
    </div>
</template>
<style lang="scss" scoped></style>
