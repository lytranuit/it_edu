import { ref, computed } from "vue";
import { defineStore } from "pinia";

import { useAxios } from "../service/axios";
const { axiosinstance } = useAxios();
export const useResources = defineStore("resources", () => {
  const list_group = ref([]);
  const list_topic = ref([]);
  const list_exam = ref([]);
  const fetchGroup = async () => {
    if (list_group.value.length) return;
    return axiosinstance
      .get("/v1/resource/groups")
      .then((res) => res.data)
      .then((response) => {
        list_group.value = response;
        return response;
      });
  };
  const fetchTopic = async () => {
    if (list_topic.value.length) return;
    return axiosinstance
      .get("/v1/resource/topics")
      .then((res) => res.data)
      .then((response) => {
        list_topic.value = response;
        return response;
      });
  };
  const fetchExam = async () => {
    if (list_exam.value.length) return;
    return axiosinstance
      .get("/v1/resource/exams")
      .then((res) => res.data)
      .then((response) => {
        list_exam.value = response;
        return response;
      });
  };
  return {
    list_group,
    list_topic,
    list_exam,
    fetchExam,
    fetchGroup,
    fetchTopic,
  };
});
