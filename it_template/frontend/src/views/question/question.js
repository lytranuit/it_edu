import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { rand } from "../../utilities/rand";
import { useAxios } from "../../service/axios";
import { useRouter } from "vue-router";
import { useConfirm } from "primevue/useconfirm";
const { axiosinstance } = useAxios();

export const useQuestionStore = defineStore("questions", () => {
  const router = useRouter();
  const confirm = useConfirm();

  const messageError = ref();
  const waiting = ref();
  const data = ref({
    tmp: rand(),
    point: 1,
    level: 2,
    topic_id: 34,
    is_random: true,
    anwsers: [
      {
        id: rand(),
      },
      {
        id: rand(),
      },
      {
        id: rand(),
      },
    ],
  });
  const anwsers = computed(() => data.value.anwsers);
  const addAnwser = () => {
    data.value.anwsers.push({
      id: rand(),
    });
  };

  const removeAnwser = (anwser) => {
    confirm.require({
      message: "Bạn có muốn xóa câu trả lời này?",
      header: "Xác nhận xóa",
      icon: "pi pi-info-circle",
      acceptClass: "p-button-danger",
      accept: () => {
        data.value.anwsers = data.value.anwsers.filter(
          (item) => item != anwser
        );
      },
      reject: () => {},
    });
  };
  const reset = () => {
    data.value.question = "";
    data.value.description = "";
    data.value.anwsers = [
      {
        id: rand(),
      },
      {
        id: rand(),
      },
      {
        id: rand(),
      },
    ];
  };
  const submit = async () => {
    waiting.value = true;
    delete data.value.list_anwser;
    await axiosinstance
      .post("/v1/question/save", data.value, {
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
        router.push("/question");
      });
    waiting.value = false;
  };
  const getData = (id) => {
    axiosinstance
      .get("/v1/question/get/" + id)
      .then((res) => res.data)
      .then((response) => {
        data.value = response;
        return response;
      });
  };

  return {
    messageError,
    data,
    waiting,
    anwsers,
    addAnwser,
    removeAnwser,
    getData,
    submit,
    reset,
  };
});
