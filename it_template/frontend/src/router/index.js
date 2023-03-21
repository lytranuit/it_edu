import { createRouter, createWebHistory } from "vue-router";
import adminLayout from "../layouts/Admin/index.vue";
import defaultLayout from "../layouts/Default.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      // redirect: "/materials/import",
      component: () => import("../views/HomeView.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Home Page - App",
      },
    },
    {
      path: "/history",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("../views/history/index.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Audittrails",
      },
    },

    {
      path: "/course",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      // component: () => import("../views/courses/List.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Khóa học",
      },
      children: [
        {
          path: "",
          component: () => import("../views/course/List.vue"),
        },
        {
          path: "learn/:id",
          component: () => import("../views/course/index.vue"),
          meta: {
            layout: defaultLayout,
            transition: "fade",
          },
        },
        {
          path: "add",
          component: () => import("../views/course/Add.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
          },
        },
        {
          path: "edit/:id",
          component: () => import("../views/course/Edit.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
          },
        },
      ],
    },
    {
      path: "/group",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("../views/group/List.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Nhóm loại đào tạo",
      },
    },
    {
      path: "/topic",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("../views/topic/List.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Topics",
      },
    },
    {
      path: "/question",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      // component: () => import("../views/question/List.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Kho câu hỏi",
      },
      children: [
        {
          path: "",
          component: () => import("../views/question/List.vue"),
        },
        {
          path: "add",
          component: () => import("../views/question/Add.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
          },
        },
        {
          path: "edit/:id",
          component: () => import("../views/question/Edit.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
          },
        },
      ],
    },
    {
      path: "/exam",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      // component: () => import("../views/question/List.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Kho đề thi",
      },
      children: [
        {
          path: "",
          component: () => import("../views/exam/List.vue"),
        },
        {
          path: "add",
          component: () => import("../views/exam/Add.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
          },
        },
        {
          path: "edit/:id",
          component: () => import("../views/exam/Edit.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
          },
        },
      ],
    },
    {
      path: "/type",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("../views/type/List.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Loại đào tạo",
      },
    },
    {
      path: "/user",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      // component: () => import("../views/user/List.vue"),
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Tài khoản",
      },
      children: [
        {
          path: "",
          component: () => import("../views/user/List.vue"),
        },
        {
          path: "add",
          component: () => import("../views/user/Add.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
            title: "Tạo tài khoản",
          },
        },
        {
          path: "edit/:id",
          component: () => import("../views/user/Edit.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
          },
        },
      ],
    },

    {
      path: "/member",
      meta: {
        layout: adminLayout,
        transition: "fade",
        title: "Thông tin tài khoản",
      },
      children: [
        {
          path: "",
          component: () => import("../views/member/Details.vue"),
        },
        {
          path: "changepassword",
          component: () => import("../views/member/ChangePassword.vue"),
          meta: {
            layout: adminLayout,
            transition: "fade",
            title: "Đổi mật khẩu",
          },
        },
      ],
    },

    {
      path: "/:pathMatch(.*)*",
      component: () => import("../views/404.vue"),
      meta: { layout: defaultLayout },
    },
  ],
});
router.beforeEach((toRoute, fromRoute, next) => {
  const title = toRoute.meta && toRoute.meta.title ? toRoute.meta.title : "App";
  document.title = title;
  next();
});
export default router;
