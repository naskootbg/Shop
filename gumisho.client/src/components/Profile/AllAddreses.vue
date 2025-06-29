<script setup>
  import { ref } from 'vue';
  import { useUserStore } from '@/stores/useUserStore';
  import Modal from '@/components/ModalWarn.vue';
  import ModalEdit from '@/components/ModalEdit.vue';
  import { DeleteAddress } from '@/api/adress.js';

  const userStore = useUserStore();
  const displayModal = ref(false);
  const displayEdit = ref(false);
  const addressToDelete = ref();
  const addressToEdit = ref();


  async function DeleteThisAddress() {
    displayModal.value = false;
    await DeleteAddress(addressToDelete.value);
    
    userStore.adresses = userStore.adresses.filter(function (el) { return el.id != addressToDelete.value; }); 
    await userStore.onEnter();
   
  };
  function DeleteConfirm(id) {
    displayModal.value = true;
    addressToDelete.value = id;
   // alert(addressToDelete.value);
  };
  function Close() {
    displayModal.value = false;
    displayEdit.value = false;
  };

  function Edit(addressObj) {
    
    addressToEdit.value = addressObj;
    displayEdit.value = !displayEdit.value;
  };
  function EditClose() {
    displayEdit.value = false;
  }
  function EditSave() {
    displayEdit.value = false;
  }
</script>
<template>
  <h3>Моите Адреси</h3>
  <div v-if="userStore.adresses.length > 0" class="addresses-container">

    <article v-for="address in userStore.adresses">


      <header>
        <p>Адрес: {{address.address}} </p>
      </header>
      <p>Имена: {{address.fullName}} </p>
      <p>Телефон: {{address.phone}}</p>

      <footer>
        <button @click="Edit(address)"> ✎ </button>
        <button @click="DeleteConfirm(address.id)" class="btn-danger"> ✖ </button>
      </footer>


      

      <footer>
        

      </footer>

    </article>
    <Modal v-if="displayModal" saveText="Да" closeText="Не" content="Сигурни ли сте, че искате да изстриете този адрес? Всички поръчки свързани с него ще бъдат изтрити!" :danger=true @save="DeleteThisAddress" @close="Close" />
    <ModalEdit v-if="displayEdit" :address="addressToEdit" @save="EditSave" @close="EditClose" />
      
        
        

  </div>
  <h5 v-else>Все още нямате добавени адреси</h5>
  
</template>
<style scoped>
  footer button {
    width: 5.5rem;
  }
  article {
    border: 1px solid #ddd;
    border-radius: 5px;
    font-size: .7rem;
  }

  .selected-address {
    margin-top: 10px;
    padding: 10px;
    background: #f8f8f8;
    border: 1px solid #ddd;
    border-radius: 5px;
    font-size: .7rem;
    
  }
  .addresses-container {
    display: grid;
    grid-template-columns: 50% 50%;
   
  }

  button {
    max-width: 10rem;
    height: 50px;
  }

  .btn-danger {
    background-color: red;
    float: right;
  }
  .input {
    height: 50px;
  }
</style>
