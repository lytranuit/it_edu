
<template>
    <select class="form-control form-control-sm" v-model="value">
        <optgroup v-for="item of list_group" :label="item.name">
            <option v-for="t of item.types" :value="t.id">{{ t.name }}</option>
        </optgroup>
    </select>
</template>
<script setup>
import { useResources } from '../../stores/resources'
import { storeToRefs } from 'pinia'
import { onMounted, computed } from 'vue';
const store = useResources();
const { list_group } = storeToRefs(store)
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
    store.fetchGroup();
});
</script>