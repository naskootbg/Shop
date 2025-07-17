<script setup>
  import { useUserStore } from '@/stores/useUserStore';


  const userStore = useUserStore();


  async function Logout() {
    await userStore.LogOut();
    location.reload();
  }

</script>
<template>
  <nav>
    <ul>
      <li>
        <div class="logo">
          <router-link :to="{ name: 'home' }">
            <img src="/logo.webp" alt="Evtinoo Logo" fetchpriority="high" />
          </router-link>
        </div>
      </li>

      <li class="header"><h1 class="main-title">Най-добри цени на хиляди продукти</h1></li>

      <li class="nav-links">
        <router-link to="/" class="card">🏠</router-link>
        <router-link to="/card" class="card">🛒</router-link>
        <router-link v-if="userStore.isUserLogged" to="/profile" class="card">👤</router-link>
        <router-link v-else to="/login" class="card">👤</router-link>
      </li>
    </ul>
  </nav>
</template>

<style scoped>
  nav {
    max-width: 100%;
    margin: auto;
  
    background: white;
    text-align: center;
  }

  .card {
    font-size: 1.6rem;
  }

  .logo {
    width: 100%;
    text-align: center;
  }

    .logo img {
      width: 260px;
      height: auto;
    }

  .main-title {
    font-size: 1rem;
    text-transform: uppercase;
    text-align: center;
    margin: 0.1rem 0;
  }

  ul {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    flex-wrap: wrap;
    gap: .1rem;
    
  }

  .nav-links {
    display: flex;
    gap: 1rem;
  }

  /* 📱 Mobile styles */
  @media (max-width: 768px) {
    ul {
      flex-direction: column;
      align-items: center;
      padding: 0px;
    
    }
    .header {
      margin-top: -2rem;
    }
    .logo img {
      width: 200px;
      margin-top: -1rem;
    }

    .nav-links {
      flex-direction: row;
      justify-content: center;
      margin-top: -2rem;
    }
  }
</style>
