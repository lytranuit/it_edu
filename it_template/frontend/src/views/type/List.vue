<template>
    <div class="row clearfix">
        <Toast />
        <div class="col-12">
            <h5 class="card-header drag-handle">
                <Button label="Tạo mới" icon="pi pi-plus" class="p-button-success p-button-sm mr-2"
                    @click="openNew"></Button>
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

                                <div v-if="col.data == 'color'">
                                    <input type="color" :value="slotProps.data[col.data]" disabled />
                                </div>
                                <div v-else v-html="slotProps.data[col.data]"></div>
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

        <Dialog v-model:visible="productDialog" :header="headerForm" :modal="true" class="p-fluid">

            <div class="row mb-2">
                <div class="field col">
                    <label for="name">Tên <span class="text-danger">*</span></label>
                    <input id="name" class="form-control form-control-sm" v-model.trim="model.name" required="true"
                        :class="{ 'p-invalid': submitted && !model.name }" />
                    <small class="p-error" v-if="submitted && !model.name">Required.</small>
                </div>
                <div class="field col">
                    <label for="name">Ký hiệu <span class="text-danger">*</span></label>
                    <input id="name" class="form-control form-control-sm" v-model.trim="model.symbol"
                        :class="{ 'p-invalid': submitted && !model.symbol }" />
                    <small class="p-error" v-if="submitted && !model.symbol">Required.</small>
                </div>
                <div class="field col">
                    <label for="name">Màu sắc</label>
                    <input type="color" class="form-control form-control-sm" v-model.trim="model.color" />
                </div>
                <div class="field col">
                    <label for="name">Nhóm</label>
                    <SelectGroup v-model:value="model.group_id"></SelectGroup>
                </div>
            </div>

            <template #footer>
                <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="hideDialog"></Button>
                <Button label="Save" icon="pi pi-check" class="p-button-text" @click="saveProduct"></Button>
            </template>
        </Dialog>

        <Dialog v-model:visible="deleteProductDialog" header="Xác nhận" :modal="true">
            <div class="confirmation-content">
                <i class="pi pi-exclamation-triangle mr-3" style="font-size: 2rem" />
                <span v-if="model">Bạn có muốn xóa <b>{{ model.name }}</b> này không?</span>
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
import { onMounted, ref, watch, computed } from 'vue';
import Loading from '../../components/Loading.vue';
import { useAxios } from "../../service/axios";
import Treeselect from 'vue3-acies-treeselect';
// import the styles
import 'vue3-acies-treeselect/dist/vue3-treeselect.css';
// import the component

import DataTable from 'primevue/datatable';
import { FilterMatchMode } from 'primevue/api';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';
import ColorPicker from 'primevue/colorpicker';
import SelectGroup from '../../components/Select/SelectGroup.vue'

import Toast from 'primevue/toast';
import { useToast } from "primevue/usetoast";
const toast = useToast();

const { axiosinstance } = useAxios();
////Datatable
const datatable = ref();
const columns = ref([
    {
        id: 0,
        label: "ID",
        data: "id",
        className: "text-center",
        filter: true,
    },
    {
        id: 1,
        label: "Tên",
        data: "name",
        className: "text-center",
        filter: true,
    },
    {
        id: 2,
        label: "Ký hiệu",
        data: "symbol",
        className: "text-center",
        filter: true,
    },
    {
        id: 3,
        label: "Màu sắc",
        data: "color",
        className: "text-center",
    }
])
const filters = ref({
    'id': { value: null, matchMode: FilterMatchMode.CONTAINS },
    'name': { value: null, matchMode: FilterMatchMode.CONTAINS },
    'symbol': { value: null, matchMode: FilterMatchMode.CONTAINS }
})
const totalRecords = ref(0);
const loading = ref(true);
const showing = ref([]);
const column_cache = "columns_type"; ////
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
const old_key = ref();
const submitted = ref();
const headerForm = ref("");
///Control
const productDialog = ref();
const deleteProductsDialog = ref();
const deleteProductDialog = ref();
const waiting = ref(false);

////Data table
const loadLazyData = () => {
    loading.value = true;
    axiosinstance.post("/v1/type/table", lazyParams.value, {
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

///Form
const valid = () => {
    if (!model.value.name.trim())
        return false;
    if (!model.value.symbol.trim())
        return false;
    return true;
}
const saveProduct = () => {
    submitted.value = true;
    if (!valid())
        return;
    waiting.value = true;
    model.value.old_key = old_key.value;
    // for (let key in model.value) {
    //     if (model.value[key] == null) model.value[key] = '';
    // }
    axiosinstance.post("/v1/type/save", model.value, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    }).then((res) => {
        return res.data;
    }).then((res) => {
        waiting.value = false;
        if (res.success) {
            if (model.value.old_key != null) {
                //edit
                toast.add({ severity: 'success', summary: 'Thành công', detail: 'Cập nhật ' + model.value.name + ' thành công', life: 3000 });
            }
            else {
                toast.add({ severity: 'success', summary: 'Thành công', detail: 'Tạo mới thành công', life: 3000 });
            }
        } else {
            toast.add({ severity: 'error', summary: 'Lỗi', detail: res.message, life: 3000 });
        }
        loadLazyData();
        model.value = {};
        productDialog.value = false;
    });


};
const editProduct = (m) => {
    old_key.value = m.id;
    headerForm.value = m.name;
    model.value = { ...m };
    productDialog.value = true;
};
const openNew = () => {
    model.value = { color: "#16A34A" };
    old_key.value = null;
    headerForm.value = "Tạo mới";
    submitted.value = false;
    productDialog.value = true;
}
const confirmDeleteSelected = () => {
    deleteProductsDialog.value = true;
}
const confirmDeleteProduct = (m) => {
    model.value = m;
    deleteProductDialog.value = true;
}
const hideDialog = () => {
    productDialog.value = false;
    submitted.value = false;
}
const deleteProduct = () => {
    waiting.value = true;
    axiosinstance.post("/v1/type/remove", { item: [model.value.id] }, {
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
    let items = selectedProducts.value.map((item) => {
        return item.id;
    })
    waiting.value = true;
    axiosinstance.post("/v1/type/remove", { item: items }, {
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