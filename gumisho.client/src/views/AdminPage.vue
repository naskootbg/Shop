<script setup>
  import { ref } from 'vue';
  
  import Services from '@/components/Admin/Services.vue';
 
  import { useAdminStore } from '@/stores/useAdminStore';


  const adminStore = useAdminStore();
  const showOrders = ref(false);
  const showOrdersDrivers = ref(false);
  const showRoles = ref(false);
  const showDrivers = ref(false);
  const showServices = ref(false);
  

  function giveOrders() {
    showOrders.value = !showOrders.value;
    showOrdersDrivers.value = false;
    showRoles.value = false;
    showDrivers.value = false;
    showServices.value = false;
  }
  function giveOrdersDrivers() {
    showOrdersDrivers.value = !showOrdersDrivers.value;
    showOrders.value = false;
    showRoles.value = false;
    showDrivers.value = false;
    showServices.value = false;
  }
  function giveRoles() {
    showRoles.value = !showRoles.value;
    showOrdersDrivers.value = false;
    showOrders.value = false;
    showDrivers.value = false;
    showServices.value = false;
  }
  function giveDrivers() {
    showDrivers.value = !showDrivers.value;
    showOrdersDrivers.value = false;
    showOrders.value = false;
    showRoles.value = false;
    showServices.value = false;
  }

  function giveServices() {
    showServices.value = !showServices.value;
    showDrivers.value = false;
    showOrdersDrivers.value = false;
    showOrders.value = false;
    showRoles.value = false;
  }
</script>

<template>
 
 


</template>

<style scoped>
   
 
</style>
