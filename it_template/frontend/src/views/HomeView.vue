
<template>
  <main>
    <div class="row">
      <div class="col-lg-3" v-for="(course, index) in courses">
        <RouterLink :to="'/course/learn/' + course.id">
          <div class="card">
            <img :src="course.image_url" alt="" class="mx-auto d-block mb-3 w-100">
            <div class="card-body">
              <div class="text-center project-card d-block">
                <!-- <span class="badge badge-soft-success font-11">Marketing</span> -->
                <h3 class="project-title" :title="course.title">{{ course.title }}</h3>
                <p class="text-muted">
                  <b class="text-secondary font-14">Mô tả :</b>
                  {{ course.description }}
                </p>
                <div class="progress mt-2" style="height:5px;">
                  <div class="progress-bar bg-secondary" role="progressbar" style="width: 92%;" aria-valuenow="92"
                    aria-valuemin="0" aria-valuemax="100"></div>
                </div>
                <h4>
                  90%
                </h4>
              </div>
            </div><!--end card-body-->
          </div><!--end card-->
        </RouterLink>
      </div><!--end col-->
    </div>
  </main>
</template>
<script setup>
import { onMounted, ref } from 'vue';
import { useAxios } from '../service/axios';
import { useRouter } from "vue-router";

const router = useRouter();
const { axiosinstance } = useAxios();
const courses = ref([]);

onMounted(() => {
  axiosinstance.get('/v1/resource/courses').then((res) => res.data).then((res) => {
    courses.value = res;
  });
})
</script>