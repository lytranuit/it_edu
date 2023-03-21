import { ref, computed } from "vue";
import { defineStore } from "pinia";

import { useAxios } from "../service/axios";
import { useRouter } from "vue-router";
import { useConfirm } from "primevue/useconfirm";
export const useCoursesStore1 = defineStore("courses1", () => {
  const { axiosinstance } = useAxios();
  const router = useRouter();
  const confirm = useConfirm();

  const waiting = ref();
  const visibleLecture = ref();
  const currentLesson = ref({});
  const activeIndex = ref([0]);
  const course = ref({
    title: "Thành thạo Microsoft PowerBI để Trực quan hóa và Phân tích dữ liệu",
    code: "PBIG01",
    chapters: [],
  });
  const personInfo = ref({
    date_pass: null,
    list_lesson_pass: [],
  });
  const chapterList = computed(() => {
    return course.value.chapters;
  });
  const lessonList = computed(() => {
    var lessons = [];
    for (let chapter of chapterList.value) {
      lessons = [...lessons, ...chapter.lessons];
    }
    return lessons;
  });
  // const chapterList = ref([
  //   {
  //     title: "Module #1 Title",
  //     active: true,
  //     lessons: [
  //       {
  //         id: "lesson-1",
  //         title: "Lesson #1 Title",
  //         type: "video",
  //         description: "",
  //         is_complete: false,
  //         active: false,
  //         nextLesson: "lesson-2",
  //       },
  //       {
  //         id: "lesson-2",
  //         title: "Lesson #2 Title",
  //         type: "video",
  //         description: "",
  //         is_complete: false,
  //         active: false,
  //         nextLesson: "lesson-3",
  //       },
  //       {
  //         id: "lesson-3",
  //         title: "Lesson #3 Title",
  //         type: "video",
  //         description: "",
  //         is_complete: false,
  //         active: false,
  //         nextLesson: "lesson-4",
  //       },
  //     ],
  //   },
  // ]);
  const totalLesson = computed(() => {
    let total = 0;
    for (let m of chapterList.value) {
      total += m.lessons.length;
    }
    return total;
  });
  const percentageCompleted = computed(() => {
    let total = totalLesson.value;
    let list_lesson_pass = personInfo.value.list_lesson_pass || [];
    list_lesson_pass = [...new Set(list_lesson_pass)];
    let total_lesson_pass = list_lesson_pass.length;
    return Math.round((total_lesson_pass / total) * 100);
  });
  const getData = async (id) => {
    return axiosinstance
      .get("/v1/course/get/" + id)
      .then((res) => res.data)
      .then((res) => {
        course.value = res;
        return getPersonInfo(res.id);
      });
  };

  const getPersonInfo = async (course_id) => {
    return axiosinstance
      .get("/v1/course/personInfo?course_id=" + course_id)
      .then((res) => res.data)
      .then((res) => {
        personInfo.value = res;
        return res;
      });
  };
  const setCurrentLesson = (lesson_id) => {
    currentLesson.value = lessonList.value.find((item) => {
      return item.id == lesson_id;
    });
    var index = chapterList.value.findIndex((item) => {
      return item.id == currentLesson.value.chapter_id;
    });
    activeIndex.value = [index];
  };
  const icon_class_lesson = (lesson) => {
    let lesson_id = lesson.id;
    let list_lesson_pass = personInfo.value.list_lesson_pass;
    if (list_lesson_pass.indexOf(lesson_id) != -1) {
      return "fas fa-check-circle text-success";
    } else if (lesson.type == 1) {
      return "fas fa-font text-danger";
    } else if (lesson.type == 2) {
      return "fas fa-video text-danger";
    } else if (lesson.type == 3) {
      return "fab fa-youtube text-danger";
    } else if (lesson.type == 4) {
      return "fas fa-file text-danger";
    } else if (lesson.type == 5) {
      return "fas fa-question text-danger";
    }
    return "";
  };
  const toogle_lecture = () => {
    visibleLecture.value = !visibleLecture.value;
  };
  const completeLesson = async () => {
    personInfo.value.list_lesson_pass.push(currentLesson.value.id);
    return await axiosinstance
      .post(
        "/v1/course/completeLesson/" + currentLesson.value.id,
        { percent_pass: percentageCompleted.value },
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
      .then((res) => res.data);
  };
  const clickLesson = async (lesson) => {
    if (waiting.value == true) return;

    waiting.value = true;
    var videoType = [2, 3, 5];
    if (videoType.indexOf(currentLesson.value.type) == -1) {
      await completeLesson();
      router.push(
        "/course/learn/" + course.value.id + "?lesson_id=" + lesson.id
      );
    } else {
      confirm.require({
        message:
          "Bạn có chưa hoàn thành bài học này! Bạn vẫn muôn tiếp tục tới bài học khác?",
        header: "Xác nhận",
        icon: "pi pi-info-circle",
        acceptClass: "p-button-danger",
        accept: () => {
          router.push(
            "/course/learn/" + course.value.id + "?lesson_id=" + lesson.id
          );
        },
      });
    }
    waiting.value = false;
  };
  const nextLesson = () => {
    var findIndex = lessonList.value.findIndex((item) => {
      return item.id == currentLesson.value.id;
    });
    if (findIndex < lessonList.value.length - 1) {
      var nextId = lessonList.value[findIndex + 1].id;
      router.push("/course/learn/" + course.value.id + "?lesson_id=" + nextId);
      return;
    }

    ////Chúc mừng hoàn thành khóa học
  };
  const endVideoYT = (e) => {
    if (e.data == 0) {
      completeLesson();
      nextLesson();
    }
  };
  const endVideo = () => {
    completeLesson();
    nextLesson();
  };
  return {
    waiting,
    visibleLecture,
    activeIndex,
    personInfo,
    percentageCompleted,
    currentLesson,
    course,
    chapterList,
    lessonList,
    totalLesson,
    getData,
    endVideo,
    endVideoYT,
    icon_class_lesson,
    setCurrentLesson,
    toogle_lecture,
    clickLesson,
  };
});
