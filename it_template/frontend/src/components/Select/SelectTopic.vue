
<template>
    <select class="form-control form-control-sm" v-model="value" >
        <option v-for="item of list_topic" :value="item.id">{{ item.title }}</option>
    </select>
</template>
<script setup>
import { useResources } from '../../stores/resources'
import { storeToRefs } from 'pinia'
import { onMounted, computed } from 'vue';
const store = useResources();
const { list_topic } = storeToRefs(store)
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
    store.fetchTopic();
});
</script>