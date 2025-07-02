<script setup>
import { useUserStore } from '@/stores/useUserStore';


const userStore = useUserStore();
const links = [
    { name: 'home', label: 'НАЧАЛО' },
  { name: 'login', label: '👤 ВХОД' },
  { name: 'join', label: '👤 РЕГИСТРАЦИЯ' },

]

 async function Logout(){
    await userStore.LogOut();
    location.reload();
 }

</script>

<template>

        <ul>
            <li><div class="logo"><img src="/logo.jpg" /></div></li>
<li><h1>Най-добри цени на хиляди продукти</h1></li>
        </ul>

        <ul v-if="!userStore.isUserLogged">
            <li v-for="link in links" :key="link.name">
                <router-link v-slot="{ isActive }" :to="{ name: link.name }">
                    <button type="button" :class="[isActive ? 'primary' : 'primary outline']">
                        {{ link.label }}
                    </button>
                </router-link>
            </li>
        </ul>
        <ul v-else>
          <li>
            <router-link v-slot="{ isActive }" :to="{ name: 'home' }">
              <button type="button" :class="[isActive ? 'primary' : 'primary outline']">
                НАЧАЛО
              </button>
            </router-link>
          </li>
          <li>
            <router-link v-slot="{ isActive }" :to="{ name: 'profile' }">
              <button type="button" :class="[isActive ? 'primary' : 'primary outline']">
                👤 ПРОФИЛ
              </button>
            </router-link>
          </li>
       
          <li>
            <router-link v-if="userStore.isAdmin" v-slot="{ isActive }" :to="{ name: 'admin' }">
              <button type="button" :class="[isActive ? 'primary' : 'primary outline']">
                АДМИН
              </button>
            </router-link>
          </li>
          <li>
            <button @click="Logout()" type="button" class="danger">
              ИЗХОД
            </button>


          </li>
        </ul>

</template>

<style scoped>
.logo{
    width: 80%;
}
h1 {
    font-size: 1rem;
    text-transform: uppercase;
}

ul {
    display: flex;
    gap: 1rem;
}
.danger{
    background-color: red;
}
button{
    font-size: .9rem;
}
</style>
