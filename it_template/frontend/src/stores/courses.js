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
    list_lesson_pass: ["lesson-1", "lesson-2"],
    current_lesson: "lesson-3",
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
    let total_lesson_pass = list_lesson_pass.length;
    return Math.round((total_lesson_pass / total) * 100);
  });
  const getData = async (id) => {
    return axiosinstance
      .get("/v1/course/get/" + id)
      .then((res) => res.data)
      .then((res) => {
        course.value = res;
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
    let current_lesson = personInfo.value.current_lesson;
    if (list_lesson_pass.indexOf(lesson_id) != -1) {
      return "fas fa-check-circle text-success";
    } else if (current_lesson == lesson_id) {
      return "fas fa-spinner fa-spin text-danger";
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
  const clickLesson = async (lesson) => {
    if (waiting.value == true) return;

    waiting.value = true;
    var videoType = [2, 3];
    console.log(currentLesson.value.type);
    if (videoType.indexOf(currentLesson.value.type) == -1) {
      await axiosinstance
        .post("/v1/course/completeLesson/" + currentLesson.value.id)
        .then((res) => res.data)
        .then((res) => {
          router.push(
            "/course/learn/" + course.value.id + "?lesson_id=" + lesson.id
          );
        });
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
    icon_class_lesson,
    setCurrentLesson,
    toogle_lecture,
    clickLesson,
  };
});
