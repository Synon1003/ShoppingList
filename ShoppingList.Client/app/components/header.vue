<template>
  <h1 class="text-2xl text-center font-twinkle-star mb-6">{{text}}</h1>
</template>

<script setup>
const props = defineProps({
  text: {
    type: String,
    required: true
  }
})
</script>