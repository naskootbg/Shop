<script setup>
  import { ref, onMounted } from "vue";
  import { GetSettings, UpdateSettings } from "@/api/mail.js";
  import Editor from '@/components/Editor.vue';

  const data = ref({});
  const server = ref('');
  const port = ref('');
  const user = ref('');
  const pass = ref('');
  const smpt = ref(false);
  onMounted(async () => {
    data.value = await GetSettings();

  });
  async function SaveData() {
    await UpdateSettings(server.value, port.value, user.value, pass.value);
    location.reload();
  }
  function showSMTP(){
    smpt.value = !smpt.value;

  }
</script>
<template>
  <h1>Редактор</h1>
  <div class="edit-div"><Editor /></div>
  <hr />
  <button @click="showSMTP">SMTP настройки</button>
  <div v-if="smpt">
    <h1>SMTP</h1>

    <p>Сървър: {{data.smtpServer}}/Порт: {{data.smtpPort}}/Потребител: {{data.smtpUser}}/</p>

    <fieldset role="group">

      <input v-model="server" type="text" name="server" placeholder="Сървър" />

      <input v-model="port" type="text" name="port" placeholder="Порт" />
    </fieldset>
    <fieldset role="group">

      <input v-model="user" type="text" name="user" placeholder="Потребител" />

      <input v-model="pass" type="text" name="pass" placeholder="Парола" />
      <button @click="SaveData">Запази</button>
    </fieldset>

  </div>
</template>


<style scoped>
  

</style>
