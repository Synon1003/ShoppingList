<template>
  <div v-if="isLoading" class="text-center p-6">
    <span class="loading loading-ring loading-lg text-primary"></span>
  </div>
</template>

<script setup>
const props = defineProps({
  isLoading: {
    type: Boolean,
    required: true
  }
})
</script>