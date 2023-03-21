
<template>
    <select class="form-control form-control-sm" v-model="value">
        <option v-for="item of list_exam" :value="item.id">{{ item.title }}</option>
    </select>
</template>
<script setup>
import { useResources } from '../../stores/resources'
import { storeToRefs } from 'pinia'
import { onMounted, computed } from 'vue';
const store = useResources();
const { list_exam } = storeToRefs(store)
const props = defineProps({
    value: {},
    multiple: { type: Boolean, default: true }
})
const emit = defineEmits(['update:value'])
const value = computed({
    get() {
        return props.value;
    },
    set(value) {
        emit("update:value", value);
    },
});
onMounted(() => {
    store.fetchExam();
});
</script>