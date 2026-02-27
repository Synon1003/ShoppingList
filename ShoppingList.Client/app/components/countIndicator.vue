<template>
    <span class="text-sm font-medium">{{ totalCount }} registered {{ totalCount == 1 ? 'item.' : 'items.' }}</span>
</template>

<script setup>
const props = defineProps({
    totalCount: {
        type: Number,
        required: true
    }
})
</script>