<template>
    <div class="row clearfix">
        <Toast />
        <div class="col-12">
            <h5 class="card-header drag-handle">
                <Button label="Tạo mới" icon="pi pi-plus" class="p-button-success p-button-sm mr-2"></Button>
                <Button label="Xóa" icon="pi pi-trash" class="p-button-danger p-button-sm" @click="confirmDeleteSelected"
                    :disabled="!selectedProducts || !selectedProducts.length"></Button>
            </h5>
            <section class="card card-fluid">
                <div class="card-body" style="overflow:auto;position: relative;">
                    <DataTable showGridlines :value="datatable" :lazy="true" ref="dt" :paginator="true"
                        v-model:selection="selectedProducts" class="p-datatable-customers" :rows="10"
                        :totalRecords="totalRecords" @page="onPage($event)" :rowHover="true" :loading="loading"
                        responsiveLayout="scroll" :resizableColumns="true" columnResizeMode="expand"
                        v-model:filters="filters" filterDisplay="menu">
                        <template #header>
                            <div style="width: 200px;">
                                <treeselect :options="columns" v-model="showing" multiple :limit="0"
                                    :limitText="(count) => 'Hiển thị: ' + count + ' cột'">
                                </treeselect>
                            </div>
                        </template>

                        <template #empty>
                            <div class="text-center">
                                Không có dữ liệu.
                            </div>
                        </template>
                        <Column selectionMode="multiple" style="width: 3rem" :exportable="false"></Column>
                        <Column v-for="col of selectedColumns" :field="col.data" :header="col.label" :key="col.data"
                            :showFilterMatchModes="false">
                            <template #body="slotProps">
                                <div v-html="slotProps.data[col.data]"></div>
                            </template>
                            <template #filter="{ filterModel, filterCallback }" v-if="col.filter == true">
                                <InputText type="text" v-model="filterModel.value" @keydown.enter="filterCallback()"
                                    class="p-column-filter" />

                            </template>
                        </Column>

                        <Column style="width:1rem">
                            <template #body="slotProps">
                                <a class="p-link text-warning mr-2 font-16" @click="editProduct(slotProps.data)"><i
                                        class='pi pi-pencil'></i></a>
                                <a class="p-link text-danger font-16" @click="confirmDeleteProduct(slotProps.data)"><i
                                        class='pi pi-trash'></i></a>
                            </template>
                        </Column>
                    </DataTable>
                </div>
            </section>
        </div>

        <Dialog v-model:visible="deleteProductDialog" header="Xác nhận" :modal="true">
            <div class="confirmation-content">
                <i class="pi pi-exclamation-triangle mr-3" style="font-size: 2rem" />
                <span v-if="model">Bạn có muốn xóa <b>{{ model.id }}</b> này không?</span>
            </div>
            <template #footer>
                <Button label="Không" icon="pi pi-times" class="p-button-text"
                    @click="deleteProductDialog = false"></Button>
                <Button label="Đồng ý" icon="pi pi-check" class="p-button-text" @click="deleteProduct"></Button>
            </template>
        </Dialog>

        <Dialog v-model:visible="deleteProductsDialog" header="Xác nhận" :modal="true">
            <div class="confirmation-content">
                <i class="pi pi-exclamation-triangle mr-3" style="font-size: 2rem"></i>
                <span>Bạn có muốn xóa tất cả những mục đã chọn không?</span>
            </div>
            <template #footer>
                <Button label="Không" icon="pi pi-times" class="p-button-text"
                    @click="deleteProductsDialog = false"></Button>
                <Button label="Đồng ý" icon="pi pi-check" class="p-button-text" @click="deleteSelectedProducts"></Button>
            </template>
        </Dialog>
        <Loading :waiting="waiting"></Loading>
    </div>
</template>

<script setup>
import { onMounted, ref, watch, computed, provide } from 'vue';
import Loading from '../../components/Loading.vue';
import { useAxios } from "../../service/axios";
import Treeselect from 'vue3-acies-treeselect';
// import { storeToRefs } from 'pinia'
// import the styles
import 'vue3-acies-treeselect/dist/vue3-treeselect.css';

// import the component

import DataTable from 'primevue/datatable';
import { FilterMatchMode } from 'primevue/api';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';

import Toast from 'primevue/toast';
import { useToast } from "primevue/usetoast";
const toast = useToast();

const { axiosinstance } = useAxios();

////Datatable
const datatable = ref();
const columns = ref([
    {
        id: 0,
        label: "id",
        data: "id",
        className: "text-center"
    },
    {
        id: 1,
        label: "Tiêu đề",
        data: "title",
        className: "text-center",
        filter: true,
    },
    {
        id: 2,
        label: "Mã khóa học",
        data: "code",
        className: "text-center",
        filter: true,
    }
])
const filters = ref({
    'title': { value: null, matchMode: FilterMatchMode.CONTAINS },
    'code': { value: null, matchMode: FilterMatchMode.CONTAINS }
})
const totalRecords = ref(0);
const loading = ref(true);
const showing = ref([]);
const column_cache = "columns_courses"; ////
const first = ref(0);
const rows = ref(10);
const draw = ref(0);
const selectedProducts = ref();
const selectedColumns = computed(() => {
    return columns.value.filter(col => showing.value.includes(col.id));
});
const lazyParams = computed(() => {
    let data_filters = {};
    for (let key in filters.value) {
        data_filters[key] = filters.value[key].value;
    }
    return {
        draw: draw.value,
        start: first.value,
        length: rows.value,
        filters: data_filters
    }
});
const dt = ref(null);

////Form
const model = ref();
///Control
const fileDialog = ref();
const deleteProductsDialog = ref();
const deleteProductDialog = ref();
const waiting = ref(false);

////Data table
const loadLazyData = () => {
    loading.value = true;
    axiosinstance.post("/v1/course/table", lazyParams.value, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    }).then((res) => {
        return res.data;
    }).then((res) => {
        // console.log(res);
        datatable.value = res.data;
        totalRecords.value = res.recordsFiltered;
        loading.value = false;
    });
}
const onPage = (event) => {
    first.value = event.first;
    rows.value = event.rows;
    draw.value = draw.value + 1;
    loadLazyData();
};

const confirmDeleteSelected = () => {
    deleteProductsDialog.value = true;
}
const confirmDeleteProduct = (m) => {
    model.value = m;
    deleteProductDialog.value = true;
}
const deleteProduct = () => {
    waiting.value = true;
    axiosinstance.post("/v1/course/remove", { item: [model.value.maNSX] }, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    }).then((res) => {
        return res.data;
    }).then((res) => {
        waiting.value = false;
        if (res.success) {

            toast.add({ severity: 'success', summary: 'Thành công', detail: 'Đã xóa thành công!', life: 3000 });
        } else {
            toast.add({ severity: 'error', summary: 'Lỗi', detail: res.message, life: 3000 });
        }
        deleteProductDialog.value = false;
        model.value = {};
        loadLazyData();
    });

};
const deleteSelectedProducts = () => {
    // datatable.value = datatable.value.filter(val => !selectedProducts.value.includes(val));
    let item = selectedProducts.value.map((item) => {
        return item.maNSX;
    })
    waiting.value = true;
    axiosinstance.post("/v1/course/remove", { item: list_maNSX }, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    }).then((res) => {
        return res.data;
    }).then((res) => {
        waiting.value = false;
        if (res.success) {

            toast.add({ severity: 'success', summary: 'Thành công', detail: 'Đã xóa thành công!', life: 3000 });
        } else {
            toast.add({ severity: 'error', summary: 'Lỗi', detail: res.message, life: 3000 });
        }
        deleteProductsDialog.value = false;
        selectedProducts.value = null;
        loadLazyData();
    });
};

////Core
onMounted(() => {
    let cache = localStorage.getItem(column_cache);
    if (!cache) {
        showing.value = columns.value.map((item) => {
            return item.id;
        });
    } else {
        showing.value = JSON.parse(cache);
    }
    loadLazyData();
})
watch(showing, async (newa, old) => {
    localStorage.setItem(column_cache, JSON.stringify(newa));
})
watch(filters, async (newa, old) => {
    loadLazyData();
})
</script>