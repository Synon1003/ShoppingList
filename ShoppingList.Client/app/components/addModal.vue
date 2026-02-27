<template>
  <div class="flex">
    <button class="btn btn-primary btn-sm uppercase" onclick="addItemModal.showModal()">New Item</button>

    <dialog id="addItemModal" class="modal" @close="resetForm">
      <div class="modal-box">
        <h3 class="font-bold text-lg mb-4">New Item</h3>

        <form @submit.prevent="submitForm" class="space-y-4">
          <div>
            <input
                v-model="name"
                type="text"
                name="name"
                placeholder="name"
                maxlength="50"
                class="input input-bordered w-full"
            />
            <p v-if="errors.name" class="text-error text-sm mt-1">
                {{ errors.name }}
            </p>
          </div>

          <div>
            <textarea
                v-model="description"
                name="description"
                placeholder="description"
                maxlength="200"
                class="textarea textarea-bordered w-full"
            ></textarea>
            <p v-if="errors.description" class="text-error text-sm mt-1">
                {{ errors.description }}
            </p>
          </div>

          <div>
            <input
                v-model="price"
                type="number"
                name="price"
                placeholder="price"
                class="input input-bordered w-full"
            />
            <p v-if="errors.price" class="text-error text-sm mt-1">
                {{ errors.price }}
            </p>
          </div>

          <div class="modal-action">
            <button type="button" class="btn btn-sm" onclick="addItemModal.close()">Back</button>
            <button type="submit" class="btn btn-sm btn-primary">Save</button>
          </div>
        </form>
      </div>
    </dialog>
  </div>
</template>


<script setup>
import { ref } from 'vue';
const emits = defineEmits(['addItem']);

const name = ref('');
const description = ref('');
const price = ref('');
const errors = ref({
  name: '',
  description: '',
  price: ''
});

function resetForm() {
  name.value = '';
  description.value = '';
  price.value = '';
  errors.value = { name: '', description: '', price: '' };
}

function submitForm() {
  errors.value = { name: '', description: '', price: '' };
  let isValid = true;

  if (!name.value || name.value.length > 50)
    errors.value.name = 'Item name is required (maximum 50 characters).';

  if (!description.value || description.value.length > 200)
    errors.value.description = 'Item description is required (maximum 200 characters).';

  const priceValue = Number(price.value);
  if ((!price.value && price.value !== 0) || isNaN(priceValue)
    || priceValue < 0 || priceValue > 100000)
    errors.value.price = 'Item price is required [0, 100000].';

  isValid = Object.values(errors.value).every(err => err === '');

  if (isValid) {
    emits('addItem', {
      name: name.value,
      description: description.value,
      price: priceValue
    });

    addItemModal.close();
    resetForm();
  }
}
</script>