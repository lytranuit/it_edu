import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { rand } from "../../utilities/rand";
import { useAxios } from "../../service/axios";
import { useRouter } from "vue-router";
import { useConfirm } from "primevue/useconfirm";
const { axiosinstance } = useAxios();

export const useCoursesStore = defineStore("courses", () => {
  const router = useRouter();
  const confirm = useConfirm();

  const messageError = ref();
  const waiting = ref();
  const data = ref({
    tmp: rand(),
    image_url:
      "/private/upload/5a375cd2-1908-4784-9b7b-d470e2d63376/default/p-2.svg",
    chapters: [],
  });
  const chapterList = computed(() => {
    return data.value.chapters;
  });
  const lessonTypes = ref([
    { id: 1, label: "Văn bản" },
    { id: 2, label: "Video" },
    { id: 3, label: "Youtube" },
    { id: 4, label: "Bài giảng" },
    { id: 5, label: "Bài kiểm tra" },
  ]);
  const lessonDialog = ref();

  const model = ref();

  const reset = () => {
    data.value = {
      tmp: rand(),
      image_url:
        "/private/upload/5a375cd2-1908-4784-9b7b-d470e2d63376/default/p-2.svg",
      chapters: [],
    };
  };
  const submit = async () => {
    ///Vaild

    waiting.value = true;
    var chapters = data.value.chapters;
    delete data.value.chapters;
    var course = await axiosinstance
      .post("/v1/course/SaveCourse", data.value, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((response) => {
        return response.data;
      })
      .then((response) => {
        messageError.value = "";
        if (response.success) {
          let course = response.data;
          return course;
        } else {
          return false;
          messageError.value = response.message;
        }
      });
    if (!course) return;
    var PromiseAll = [];
    for (let chapter of chapters) {
      var lessons = chapter.lessons;
      delete chapter.lessons;
      chapter.course_id = course.id;
      chapter = await axiosinstance
        .post("/v1/course/SaveChapter", chapter, {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        })
        .then((response) => {
          return response.data;
        })
        .then((response) => {
          messageError.value = "";
          if (response.success) {
            return response.data;
          } else {
            throw Error(response.message);
            messageError.value = response.message;
          }
        });
      for (let lesson of lessons) {
        lesson.chapter_id = chapter.id;
        lesson.course_id = course.id;
        var promise = axiosinstance
          .post("/v1/course/SaveLesson", lesson, {
            headers: {
              "Content-Type": "multipart/form-data",
            },
          })
          .then((response) => {
            return response.data;
          })
          .then((response) => {
            messageError.value = "";
            if (response.success) {
              return response.data;
            } else {
              throw Error(response.message);
              messageError.value = response.message;
            }
          });
        PromiseAll.push(promise);
      }
    }
    await Promise.all(PromiseAll)
      .then(() => {
        router.push("/course");
      })
      .catch(() => {});
    waiting.value = false;
  };

  const addChapter = () => {
    data.value.chapters.push({
      title: "Chương " + (chapterList.value.length + 1) + ": ",
      lessons: [],
    });
  };

  const removeChapter = (chapter) => {
    confirm.require({
      message: "Bạn có muốn xóa Chương trình học này?",
      header: "Xác nhận xóa",
      icon: "pi pi-info-circle",
      acceptClass: "p-button-danger",
      accept: () => {
        data.value.chapters = chapterList.value.filter(
          (item) => item != chapter
        );
        if (chapter.id) {
          var chapterRemove = data.value.remove || [];
          chapterRemove.push(chapter.id);
          data.value.remove = chapterRemove;
        }
      },
      reject: () => {},
    });
  };
  const addLesson = (chapter) => {
    lessonDialog.value = true;
    let lessons = chapter.lessons || [];
    model.value = {
      type: 1,
      title: "Bài " + (lessons.length + 1) + ": ",
      chapter: chapter,
    };
    // chapter.lessons.push({ title: "Lesson" });
  };
  const editLesson = (lesson, chapter) => {
    lessonDialog.value = true;
    model.value = { ...lesson };
    model.value.chapter = chapter;
  };
  const removeLesson = (lesson, chapter) => {
    confirm.require({
      message: "Bạn có muốn xóa Bài học này?",
      header: "Xác nhận xóa",
      icon: "pi pi-info-circle",
      acceptClass: "p-button-danger",
      accept: () => {
        if (lesson.tmp) {
          ////
          var index = chapter.lessons.findIndex(function (item) {
            return item.tmp == lesson.tmp;
          });
          chapter.lessons.splice(index, 1);
        } else if (lesson.id) {
          ////
          var index = chapter.lessons.findIndex(function (item) {
            return item.id == lesson.id;
          });
          chapter.lessons.splice(index, 1);
          ////Remove
          var Remove = chapter.remove || [];
          Remove.push(lesson.id);
          chapter.remove = Remove;
        }
      },
    });
  };
  const saveLesson = () => {
    if (!model.value.title || model.value.title.trim() == "") {
      alert("Chưa nhập tiêu đề!");
      return false;
    }
    var chapter = model.value.chapter;
    delete model.value.chapter;
    if (model.value.tmp) {
      ////edit
      var index = chapter.lessons.findIndex(function (item) {
        return item.tmp == model.value.tmp;
      });
      chapter.lessons[index] = model.value;
    } else if (model.value.id) {
      ////edit
      var index = chapter.lessons.findIndex(function (item) {
        return item.id == model.value.id;
      });
      chapter.lessons[index] = model.value;
    } else {
      ///add
      model.value.tmp = rand();
      chapter.lessons.push(model.value);
    }

    lessonDialog.value = false;
  };
  const attachments = (e) => {
    model.value.attachments = e.files;
  };
  const file = (e) => {
    model.value.file = e.files.length > 0 ? e.files[0] : null;
    if (model.value.file_up) {
      removeFile(model.value.file_up);
    }
    setFileinfo();
  };
  const check_presentation = (file) => {
    var image = file.objectURL;
    var type = file.type;
    if (type.indexOf("image") == -1) {
      image = "/images/file.png";
    }
    return image;
  };
  const check_presentation_file_uploaded = (file) => {
    var image = file.url;
    var type = file.mimeType;
    if (type.indexOf("image") == -1) {
      image = "/images/file.png";
    }
    return image;
  };
  const formatSize = (bytes) => {
    if (bytes === 0) {
      return "0 B";
    }

    let k = 1000,
      dm = 3,
      sizes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"],
      i = Math.floor(Math.log(bytes) / Math.log(k));

    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i];
  };
  const getData = (id) => {
    axiosinstance
      .get("/v1/course/get/" + id)
      .then((res) => res.data)
      .then((response) => {
        data.value = response;
        return response;
      });
  };
  const removeFileAttachment = (file) => {
    model.value.attachments_up = model.value.attachments_up.filter((item) => {
      return item != file;
    });

    ////Remove
    var Remove = model.value.removeAttachments || [];
    Remove.push(file.id);
    model.value.removeAttachments = Remove;
  };
  const removeFile = (file) => {
    model.value.file_up = null;

    ////Remove
    var Remove = model.value.removeFile || [];
    Remove.push(file.id);
    model.value.removeFile = Remove;
  };
  const getInfoYoutube = (event) => {
    var link = model.value.file_url;
    var array = link.split("v=");
    var video_id = null;
    if (array.length > 1) {
      video_id = array[array.length - 1];
    } else {
      array = link.split("/");
      if (array.length > 1) {
        video_id = array[array.length - 1];
      }
    }

    var ampersandPosition = video_id.indexOf("&");
    if (ampersandPosition != -1) {
      video_id = video_id.substring(0, ampersandPosition);
    }
    GetVideoDuration(video_id);
    model.value.youtube_id = video_id;
    // console.log(video_id);
  };
  const GetVideoDuration = (video_id) => {
    axiosinstance
      .get("/v1/resource/GetVideoDuration/" + video_id)
      .then((res) => {
        return res.data;
      })
      .then((res) => {
        model.value.duration = parseInt(res);
      });
  };
  const setFileinfo = () => {
    if (model.value.type != 2) return;
    window.URL = window.URL || window.webkitURL;
    var file = model.value.file;
    var video = document.createElement("video");
    video.preload = "metadata";
    video.onloadedmetadata = () => {
      window.URL.revokeObjectURL(video.src);
      var duration = video.duration;
      model.value.duration = parseInt(duration);
    };
    video.src = URL.createObjectURL(file);
  };
  return {
    messageError,
    data,
    chapterList,
    lessonTypes,
    lessonDialog,
    model,
    waiting,
    submit,
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
    removeFile,
    formatSize,
    getData,
    reset,
    getInfoYoutube,
  };
});
