<template>
  <div class="max-w-4xl mx-auto px-4 py-10">
    <Header text="Shopping List" />

    <div class="flex items-center justify-between mb-4">
      <CountIndicator :totalCount="totalCount" />
      <AddModal @addItem="addItem" />
    </div>

    <div v-if="!loading" class="overflow-x-auto mx-auto max-w-4xl w-full">
      <ItemTable
        :items="tableItems"
        @toggleItemStatus="toggleItemStatus"
        @deleteItem="deleteItemById" />
    </div>

    <Toast
      v-if="showToast"
      :type="toastType" 
      :message="toastMessage"
    />
    <Loader :isLoading="loading"/>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

import { itemService } from '~/composables/useItemService'

import Header from './components/header.vue'
import CountIndicator from './components/countIndicator.vue'
import AddModal from './components/addModal.vue'
import ItemTable from './components/itemTable.vue'
import Toast from './components/toast.vue'
import Loader from './components/loader.vue'

let tableItems = ref([])
let totalItems = ref([])
const totalCount = ref(0)

const showToast = ref(false)
const toastType = ref('')
const toastMessage = ref('')
const loading = ref(false)


async function loadItems()
{
  loading.value = true
  const [fetchedItems, message] = await itemService.fetchItems()
  if (message != '') {
    showToastMessage('error', message)
  }
  else {
    totalItems.value = fetchedItems
    totalCount.value = totalItems.value.length
    if (totalCount.value === 0)
      showToastMessage('info', 'There are no items in the shopping list.')
  }
  loading.value = false
}

async function switchItemById(itemId)
{
  const [fetchedItem, message] = await itemService.fetchItemById(itemId)
  if (fetchedItem != null) {
    const totalIndex = totalItems.value.findIndex(item => item.id === itemId)
    const tableIndex = tableItems.value.findIndex(item => item.id === itemId)
    if (totalIndex !== -1 && tableIndex !== -1)
    {
      totalItems.value[totalIndex] = fetchedItem
      tableItems.value[tableIndex] = fetchedItem
    }
  } else {
    showToastMessage('error', message)
  }
}

async function addItem(item)
{
  const [isSuccess, message] = await itemService.createItem(item)
  if (isSuccess) {
    await loadItems()
    tableItems.value = totalItems.value
    showToastMessage('success', message)
  } else {
    showToastMessage('error', message)
  }
}

async function toggleItemStatus(itemId)
{
  const [isSuccess, message] = await itemService.patchItem(itemId)
  if (isSuccess) {
    showToastMessage('success', message)
    await switchItemById(itemId)
  } else {
    showToastMessage('error', message)
  }
}

async function deleteItemById(itemId)
{
  const [isSuccess, message] = await itemService.deleteItem(itemId)
  if (isSuccess) {
    showToastMessage('success', message)
    await loadItems()
    tableItems.value = totalItems.value
  } else {
    showToastMessage('error', message)
  }
}

function showToastMessage(type, message) {
  toastType.value = type
  toastMessage.value = message
  showToast.value = true
  setTimeout(() => { showToast.value = false }, 5000)
}

onMounted(async () =>
{
  await loadItems()
  tableItems.value = totalItems.value
})

</script>

<style scoped></style>