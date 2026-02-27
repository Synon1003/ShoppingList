<template>
  <div class="toast toast-center">
    <div v-if="type=='success'" class="alert alert-success">
        <span>{{ props.message }}</span>
    </div>
    <div v-else-if="type=='error'" class="alert alert-error">
        <span>{{ props.message }}</span>
    </div>
    <div v-else-if="type=='info'" class="alert alert-info">
        <span>{{ props.message }}</span>
    </div>

  </div>
</template>

<script setup>
const props = defineProps({
  type: {
    type: String,
    required: true,
    validator: value => ['info', 'success', 'error'].includes(value),
  },
  message: {
    type: String,
    required: true
  },
})
</script>