<template>
  <table class="table w-full">
    <thead class="bg-primary">
      <tr class="bg-base-300/20 text-neutral">
        <th>Name</th>
        <th>Description</th>
        <th>Price</th>
        <th>Updated At</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="(item, index) in items"
        :key="item.id"
        :class="item.status === 'Purchased' ? 'bg-base-300 line-through' : 'bg-transparent'">
        <td style="word-break: break-word; max-width: 150px;">{{ item.name }}</td>
        <td style="word-break: break-word; max-width: 250px;">{{ item.description }}</td>
        <td>{{ item.price }}</td>
        <td>{{ new Date(item.updatedAt).toLocaleString('hu-HU', { timeZone: 'UTC' }) }}</td>
        <td class="px-4 py-2 text-sm text-primary">
          <div class="flex gap-2">
            <button
              class="hover:cursor-pointer hover:underline"
              @click="emits('toggleItemStatus', item.id)"
            >
              {{ item.status }}
            </button>
            <span>|</span>
            <button
              class="hover:cursor-pointer hover:underline"
              @click="emits('deleteItem', item.id)"
            >
              Delete
            </button>
          </div>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script setup>
const props = defineProps({
  items: {
    type: Array,
    required: true
  },
})

const emits = defineEmits(['toggleItemStatus', 'deleteItem'])
</script>