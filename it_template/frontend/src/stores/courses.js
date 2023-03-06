import { ref, computed } from "vue";
import { defineStore } from "pinia";

export const useCoursesStore = defineStore("courses", () => {
  const currentLesson = ref();
  const courses = ref({
    title: "Thành thạo Microsoft PowerBI để Trực quan hóa và Phân tích dữ liệu",
    code: "PBIG01",
  });
  const personInfo = ref({
    date_pass: null,
    list_lesson_pass: ['lesson-1','lesson-2'],
    current_lesson: 'lesson-3',
  });
  const chapterList = ref([
    {
      title: "Module #1 Title",
      active: true,
      lessons: [
        {
          id: "lesson-1",
          title: "Lesson #1 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-2",
        },
        {
          id: "lesson-2",
          title: "Lesson #2 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-3",
        },
        {
          id: "lesson-3",
          title: "Lesson #3 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-4",
        },
      ],
    },
    {
      title: "Module #2 Title",
      active: false,
      lessons: [
        {
          id: "lesson-4",
          title: "Lesson #4 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-5",
        },
        {
          id: "lesson-5",
          title: "Lesson #5 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-6",
        },
        {
          id: "lesson-6",
          title: "Lesson #6 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-7",
        },
      ],
    },
    {
      title: "Module #3 Title",
      active: false,
      lessons: [
        {
          id: "lesson-7",
          title: "Lesson #7 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-8",
        },
        {
          id: "lesson-8",
          title: "Lesson #8 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-9",
        },
        {
          id: "lesson-9",
          title: "Lesson #9 Title",
          type: "video",
          description: "",
          is_complete: false,
          active: false,
          nextLesson: "lesson-9",
        },
      ],
    },
  ]);
  const totalLesson = computed(() => {
    let total = 0;
    for (let m of moduleList.value) {
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

  return {
    personInfo,
    percentageCompleted,
    currentLesson,
    courses,
    chapterList,
    totalLesson,
  };
});
