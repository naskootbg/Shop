<script setup>
  import { ref, onMounted } from 'vue';
  import { useAdminStore } from '@/stores/useAdminStore';
  import { AddHash, DelHash } from '@/api/feed';
  const adminStore = useAdminStore();
  onMounted(async () => {

    adminStore.onEnter();

  });
  const hashes = ref(adminStore.hashes)
  const hashName = ref('');
  const hashVal = ref('');
  async function addHash() {
    var hash = await AddHash(hashVal.value, hashName.value);
    adminStore.hashes.push(hash);
    hashVal.value = '';
    hashName.value = '';
  }
  async function delHash(id) {
    var hash = await DelHash(id);
    adminStore.hashes = adminStore.hashes.filter(h => h.id != id);
  }
</script>

<template>
 
  <fieldset role="group">
    <input v-model="hashName" type="text" name="hashName" placeholder="Име" />
    <input v-model="hashVal" type="text" name="hashVal" placeholder="Хеш" />
    <button @click="addHash">➕</button>
  </fieldset>
  <div class="data">
    <table>
      <thead><tr><th>Име</th><th>Хеш</th><th></th></tr></thead>
      <tbody><tr v-for="hash in adminStore.hashes" :key="hash.id"><td>{{hash.name}}</td><td>{{hash.hash}}</td><td><button @click="delHash(hash.id)" class="delbtn">🗑️</button></td></tr></tbody>
    </table>
  </div>
</template>

<style scoped>
   table, th, tr, td{
     font-size: .8rem;
     padding: 1px;
     text-align: center;
   }
  .data {
    width: 60%;
    align-items: center;
  }
  .delbtn {
    background: red;
    padding: 1px;
  }
</style>
