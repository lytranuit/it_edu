
<template>
  <div class="left-sidenav">
    <ul class="metismenu left-sidenav-menu">
      <li>
        <router-link to="/">
          <i class="ti-bar-chart"></i>
          <span>Tổng quan</span>
        </router-link>
      </li>
      <li>
        <a href="javascript: void(0);">
          <i class="fab fa-discourse"></i>
          <span>Khóa học</span>
          <span class="menu-arrow"><i class="mdi mdi-chevron-down"></i></span>
        </a>
        <ul class="nav-second-level" aria-expanded="false">
          <li class="nav-item">
            <a href="javascript: void(0);">
              <i class="ti-control-record"></i>
              Dành cho tôi
            </a>
          </li>
          <li class="nav-item">
            <a href="javascript: void(0);">
              <i class="ti-control-record"></i>
              Khóa đã học
            </a>
          </li>
          <li class="nav-item">
            <a href="javascript: void(0);">
              <i class="ti-control-record"></i>
              Yêu cầu khóa học
            </a>
          </li>
        </ul>
      </li>
      <li>
        <a href="javascript: void(0);">
          <i class="ti-briefcase"></i>
          <span>Thiết lập</span>
          <span class="menu-arrow"><i class="mdi mdi-chevron-down"></i></span>
        </a>
        <ul class="nav-second-level" aria-expanded="false">
          <li class="nav-item">
            <a href="javascript: void(0);">
              <i class="ti-control-record"></i>
              Khóa học
              <span class="menu-arrow"><i class="mdi mdi-chevron-down"></i></span>
            </a>
            <ul class="nav-second-level" aria-expanded="false">
              <li class="nav-item">
                <router-link to="/course">
                  <i class="ti-control-record"></i>
                  Khóa học
                </router-link>
              </li>
              <li class="nav-item">
                <router-link to="/group">
                  <i class="ti-control-record"></i>
                  Nhóm đào tạo
                </router-link>
              </li>
              <li class="nav-item">
                <router-link to="/type">
                  <i class="ti-control-record"></i>
                  Loại đào tạo
                </router-link>
              </li>
            </ul>
          </li>
          <li class="nav-item">
            <a href="javascript: void(0);">
              <i class="ti-control-record"></i>
              Trung tâm bài thi
              <span class="menu-arrow"><i class="mdi mdi-chevron-down"></i></span>
            </a>
            <ul class="nav-second-level" aria-expanded="false">
              <li class="nav-item">
                <router-link class="nav-link" to="/exam"><i class="ti-control-record"></i>Kho bài thi</router-link>
              </li>
              <li class="nav-item">
                <router-link class="nav-link" to="/question"><i class="ti-control-record"></i>Kho câu hỏi</router-link>
              </li>
              <li class="nav-item">
                <router-link class="nav-link" to="/topic"><i class="ti-control-record"></i>Topics</router-link>
              </li>
            </ul>
          </li>

        </ul>
      </li>

      <li v-if="is_admin">
        <router-link class="nav-link" to="/history">
          <i class="fas fa-history"></i>
          <span>Audittrails</span>
        </router-link>
      </li>
      <li v-if="is_admin">

        <a href="javascript: void(0);">
          <i class="ti-briefcase"></i>
          <span>Hệ thống</span>
          <span class="menu-arrow"><i class="mdi mdi-chevron-down"></i></span>
        </a>

        <ul class="nav-second-level" aria-expanded="false">
          <li class="nav-item">
            <router-link class="nav-link" to="/user"><i class="ti-control-record"></i>Tài khoản</router-link>
          </li>
        </ul>
      </li>
    </ul>
  </div>
</template>
<script setup>

import { useAuth } from '../../stores/auth';
const store = useAuth();
const is_admin = store.is_admin;
// console.log(in_groups);
// import { onMounted } from 'vue'
</script>