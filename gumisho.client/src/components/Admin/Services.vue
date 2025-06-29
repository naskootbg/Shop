<script setup>
  import { ref } from "vue";
  import { AllServices, AddService, EditService, DeleteService, ShowService } from "@/api/admin.js";
  import { useAdminStore } from '@/stores/useAdminStore';
  import { useOrderStore } from '@/stores/useOrderStore';

  const orderStore = useOrderStore();
  const adminStore = useAdminStore();

  const showForm = ref(false);
  const carpetPrice = ref("");
  const deliveryPrice = ref("");
  const title = ref("");
  const description = ref("");
  const serviceId = ref(0);

  async function EditTheService(id) {
    var current = await ShowService(id);
    showForm.value = true;
    serviceId.value = id;
    title.value = current.name;
    description.value = current.description;
    carpetPrice.value = current.priceCarpet;
    deliveryPrice.value = current.priceDelivery;
  }
  async function DeleteTheService(id) {
    
    await DeleteService(id);
    await orderStore.LoadServices();
  }
  async function ShowAddForm() {
    showForm.value = true;
    serviceId.value = 0;
    title.value = "";
    description.value = "";
    carpetPrice.value = "";
    deliveryPrice.value = "";
  };

  async function SaveEdit() {
      const form = {
        name: title.value,
        description: description.value,
        quantity: 0,
        priceCarpet: carpetPrice.value,
        priceDelivery: deliveryPrice.value,
    }

    if (serviceId.value > 0) {
      form.id = serviceId.value;
      await EditService(form);
    }
    else {
      await AddService(form);
    };
    await orderStore.LoadServices();
    showForm.value = false;
    serviceId.value = 0;
    title.value = "";
    description.value = "";
    carpetPrice.value = "";
    deliveryPrice.value = "";
  };
</script>

<template>
  <div class="addresses-container">
    <article v-for="service in orderStore.services">
      <header>
        Услуга: {{service.name}}
      </header>
      Обяснение: {{service.description}}
      <footer>
        Цена услуга: {{service.priceCarpet}}лв, Цена доставка: {{service.priceDelivery}}лв<br />
        <fieldset role="group" class="footer-btn">
          <button @click="EditTheService(service.id)">✎</button><button @click="DeleteTheService(service.id)" class="text-danger">✖</button>
        </fieldset>
      </footer>
    </article>
  </div>
  <button @click="ShowAddForm">Добави нов</button>
  <div v-if="showForm">
    <label for="title"><b>Заглавие</b></label>
    <input type="text" v-model="title" name="title" placeholder="Заглавие" />
    <label for="description"><b>Обяснение</b></label>
    <textarea v-model="description" name="description" placeholder="Обяснение"></textarea>
    <fieldset role="group" class="custom-fields">
      <input type="text" v-model="serviceId" name="serviceId" placeholder="Id" hidden />
      <label for="carpetPrice"><b class="smaller-text">Цени услуга/доставка</b></label>
      <input type="number" v-model="carpetPrice" name="carpetPrice" placeholder="Цена Услуга" />
      <input type="number" v-model="deliveryPrice" name="deliveryPrice" placeholder="Цена Доставка" autofocus />

      <button @click="SaveEdit">Запази</button>
    </fieldset>
  </div>
</template>

<style scoped>
  .custom-fields input{
    width: 9rem;
  }

  .updated {
    height: 50px;
    font-size: .8rem;
  }

  .smaller-text{
    font-size: .8rem;
  }

  .addresses-container {
    display: grid;
    grid-template-columns: 50% 50%;
  }

  .footer-btn button {
    width: 7rem;
  }

  .text-danger {
    background-color: red;
  }
</style>
