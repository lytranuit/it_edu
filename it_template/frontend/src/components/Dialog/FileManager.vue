
<template>
    <div>
        <img :src="value" class="image_feature" style="max-height: 100px; max-width: 100%; cursor:pointer;"
            @click="fileDialog = true">
        <Dialog v-model:visible="fileDialog" header="File Manager" :modal="true" style="width:50vw">
            <FileManagerComponent id="file-manager" :ajaxSettings="{
                url: '/FileManager/FileManager/FileOperations',
                getImageUrl: '/FileManager/FileManager/GetImage',
                uploadUrl: '/FileManager/FileManager/Upload',
                downloadUrl: '/FileManager/FileManager/Download'
            }" :uploadSettings="{
    allowedExtensions: '.png,.jpg',
    maxFileSize: 20 * 1000 * 1000,
    autoClose: true
}" :allowMultiSelection="false" @fileOpen="chooseFile($event)">
            </FileManagerComponent>
        </Dialog>
    </div>
</template>
<script setup>
import { storeToRefs } from 'pinia'
import { onMounted, computed, provide, ref } from 'vue';
import { FileManagerComponent, DetailsView, NavigationPane, Toolbar } from "@syncfusion/ej2-vue-filemanager";

import { useAuth } from '../../stores/auth'
import Dialog from 'primevue/dialog';
// import the styles


import "@syncfusion/ej2-base/styles/material.css";
import "@syncfusion/ej2-icons/styles/material.css";
import "@syncfusion/ej2-inputs/styles/material.css";
import "@syncfusion/ej2-popups/styles/material.css";
import "@syncfusion/ej2-buttons/styles/material.css";
import "@syncfusion/ej2-splitbuttons/styles/material.css";
import "@syncfusion/ej2-navigations/styles/material.css";
import "@syncfusion/ej2-layouts/styles/material.css";
import "@syncfusion/ej2-grids/styles/material.css";
import "@syncfusion/ej2-vue-filemanager/styles/material.css";

provide('filemanager', [DetailsView, NavigationPane, Toolbar]);
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

const store = useAuth();
const user = store.user;
const fileDialog = ref();
onMounted(() => {
});

const chooseFile = (e) => {
    var details = e.fileDetails;
    if (details.isFile) {
        var filterPath = details.filterPath.replace(/\\\\/g, "\/").replace(/\\/g, "\/");
        var name = details.name;
        var path = "/private/upload/" + user.id + filterPath + name;
        value.value = path;
        fileDialog.value = false;
    }
}
</script>