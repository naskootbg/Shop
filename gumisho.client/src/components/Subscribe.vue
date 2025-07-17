<script setup>
  import { useUserStore } from '@/stores/useUserStore';
  import { useOrderStore } from '@/stores/useOrderStore';
  import { ref, watch, computed } from 'vue'
  import { Subscribe } from '@/api/subscribe.js'

  const userStore = useUserStore();
  const orderStore = useOrderStore();
  const props = defineProps({
    tag: {
      type: String,
      default: ''
    },
    category: {
      type: String,
      default: ''
    },
    product: {
      type: Object,
      default: null
    }

  });
  const success = ref(false);
  const maxPrice = ref(0);
  const minPercent = ref(0);
  const email = ref(userStore.email);
  const user = ref(userStore.username);
  watch(() => props.product, (p) => {
    if (!p) return;

    maxPrice.value = p.price_discounted ?? 0;
    minPercent.value = p._discount ?? 0;
  }, { immediate: true });


  async function joinList() {

    await Subscribe(email.value, user.value, props.tag, props.category, props.product.product_code, maxPrice.value, minPercent.value)
    success.value = true;
  }
  //
</script>
<template>
  <div class="subscribe-box">
    <h5 v-if="props.product?.product_code?.length > 0">
      Получавай нотификации при промяна в цената на този продукт
    </h5>
    <h5 v-else>
      Получавай нотификации при добавяне на продукт за {{ props.tag }}{{ props.category }}
    </h5>

    <span class="invite" v-if="!userStore.isUserLogged">
      <router-link to="/join" class="card">Регистрирай се</router-link>
      или
      <router-link to="/login" class="card">Влез</router-link>
      за да следиш цените директно в профила си.
    </span>

    <table class="subscribe-table">
      <thead>
        <tr>
          <th v-if="!userStore.isUserLogged">Име</th>
          <th v-if="!userStore.isUserLogged">Имейл</th>
          <th>При цена под (лв)</th>
          <th>При Отстъпка над (%)</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td v-if="!userStore.isUserLogged"><input v-model="user" type="text" placeholder="Вашите Имена" /></td>
          <td v-if="!userStore.isUserLogged"><input v-model="email" type="text" placeholder="Ел. Поща" /></td>
          <td><input v-model="maxPrice" type="number" /></td>
          <td><input v-model="minPercent" type="number" /></td>
        </tr>
      </tbody>
      <tfoot>
        <tr>
          <td colspan="4">
            <button @click="joinList">Желая да получа известие</button>
          </td>
        </tr>
      </tfoot>
    </table>
  </div>
</template>

<style scoped>
  *,
  *::before,
  *::after {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
  }
  .subscribe-table th,
  .subscribe-table td {
    padding: 10px;
    border: 1px solid #ddd;
    text-align: center; /* 👈 this centers text horizontally */
    vertical-align: middle; /* 👈 optional: centers text vertically */
    font-size: .8rem;
  }
  .subscribe-box {
    background: white;
    margin-top: 1rem;
    border: 1px dotted #ccc;
    border-radius: 10px;
    padding: 20px;
    text-align: center;
  }

  .subscribe-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
  }

    .subscribe-table th,
    .subscribe-table td {
      padding: 10px;
      border: 1px solid #ddd;
    }

    .subscribe-table input {
      width: 100%;
      max-width: 200px;
      padding: 6px 10px;
      font-size: 0.9rem;
    }

    .subscribe-table tfoot td {
      text-align: center;
      padding-top: 15px;
    }

  button {
    padding: 10px 20px;
    font-size: 0.9rem;
    cursor: pointer;
    background-color: #008cff;
    color: white;
    border: none;
    border-radius: 5px;
  }
</style>
