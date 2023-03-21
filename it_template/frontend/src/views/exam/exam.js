import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { rand } from "../../utilities/rand";
import { useAxios } from "../../service/axios";
import { useRouter } from "vue-router";
import { useConfirm } from "primevue/useconfirm";
const { axiosinstance } = useAxios();

export const useStore = defineStore("exams", () => {
  const router = useRouter();
  const confirm = useConfirm();

  const messageError = ref();
  const waiting = ref();
  const data = ref({
    tmp: rand(),
    image_url:
      "/private/upload/5a375cd2-1908-4784-9b7b-d470e2d63376/default/p-2.svg",
    duration: 0,
    point_pass: 80,
    questions: [
      {
        num_question: 10,
        topic_id: null,
      },
    ],
  });
  const questions = computed(() => data.value.questions);
  const addQuestion = () => {
    data.value.questions.push({
      tmp: rand(),
      num_question: 10,
    });
  };

  const removeQuestion = (question) => {
    confirm.require({
      message: "Bạn có muốn xóa mục này?",
      header: "Xác nhận xóa",
      icon: "pi pi-info-circle",
      acceptClass: "p-button-danger",
      accept: () => {
        data.value.questions = questions.value.filter(
          (item) => item != question
        );
      },
      reject: () => {},
    });
  };
  const submit = async () => {
    waiting.value = true;
    delete data.value.list_type_question;
    await axiosinstance
      .post("/v1/exam/save", data.value, {
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
          return false;
          messageError.value = response.message;
        }
      })
      .then(() => {
        router.push("/exam");
      });
    waiting.value = false;
  };
  const reset = () => {
    data.value = {
      tmp: rand(),
      image_url:
        "/private/upload/5a375cd2-1908-4784-9b7b-d470e2d63376/default/p-2.svg",
      duration: 0,
      point_pass: 80,
      questions: [
        {
          num_question: 10,
          topic_id: null,
        },
      ],
    };
  };
  const getData = (id) => {
    axiosinstance
      .get("/v1/exam/get/" + id)
      .then((res) => res.data)
      .then((response) => {
        data.value = response;
        return response;
      });
  };
  const topic_change = (question) => {
    console.log(question);
    axiosinstance
      .get("/v1/topic/countQuestion?topic_id=" + question.topic_id)
      .then((res) => res.data)
      .then((res) => {
        question.num_topic_question = res;
        return res;
      });
  };
  return {
    messageError,
    data,
    waiting,
    questions,
    topic_change,
    addQuestion,
    removeQuestion,
    getData,
    submit,
    reset,
  };
});
