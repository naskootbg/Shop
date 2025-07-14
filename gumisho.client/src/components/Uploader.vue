<template>
  <div class="p-4 border rounded shadow w-fit">
    <h2 class="text-lg font-semibold mb-2">{{ title }}</h2>

    <input type="file" @change="handleFileChange" :accept="accept" />

    <button @click="uploadFile"
            :disabled="!selectedFile || uploading"
            class="mt-2 px-4 py-1 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50">
      {{ uploading ? 'Uploading...' : buttonText }}
    </button>

    <p v-if="message" class="mt-2 text-green-600">{{ message }}</p>
    <p v-if="error" class="mt-2 text-red-600">{{ error }}</p>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import axios from 'axios'

  const props = defineProps({
    postUrl: { type: String, required: true },         // e.g. "/logo"
    title: { type: String, default: "Upload File" },
    buttonText: { type: String, default: "Upload" },
    accept: { type: String, default: "image/*" },       // You can customize file types too
  })

  const selectedFile = ref(null)
  const uploading = ref(false)
  const message = ref('')
  const error = ref('')

  function handleFileChange(event) {
    selectedFile.value = event.target.files[0]
    message.value = ''
    error.value = ''
  }

  async function uploadFile() {
    if (!selectedFile.value) return

    const formData = new FormData()
    formData.append('file', selectedFile.value)

    uploading.value = true
    message.value = ''
    error.value = ''

    try {
      const response = await axios.post(props.postUrl, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      message.value = response.data
    } catch (err) {
      error.value = err.response?.data || 'Upload failed.'
    } finally {
      uploading.value = false
    }
  }
</script>
