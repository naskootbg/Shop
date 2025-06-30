import { createRouter, createWebHistory } from "vue-router";
import HomePage from '../views/HomePage.vue';
import AdminPage from '../views/AdminPage.vue';
import LoginPage from '../views/LoginPage.vue';
import RegisterPage from '../views/RegisterPage.vue';
import NotFound from '../views/NotFound.vue';
import Profile from '../views/Profile.vue';
import ProductDetails from '@/views/ProductDetails.vue';
import { useUserStore } from '../stores/useUserStore';

const routes = [
  { path: '/', name: 'home', component: HomePage },
  { path: '/admin', name: 'admin', component: AdminPage },
  {
    path: '/login',
    name: 'login',
    component: LoginPage,
    beforeEnter: async () => {
      const store = useUserStore();
      await store.onEnter();
      if (store.isUserLogged && store.isAdmin) return '/admin';
      if (store.isUserLogged) return '/';
    }
  },
  { path: '/join', name: 'join', component: RegisterPage },
  { path: '/profile', name: 'profile', component: Profile },
  { path: '/api/logout', name: 'logout' },

  // Product details page
  { path: '/:id/:slug', name: 'ProductDetails', component: ProductDetails, props: true },

  // SEO-friendly filter routes (should load catalog!)
  { path: '/discount/:value', name: 'ByDiscount', component: HomePage, props: true },
  { path: '/category/:category', name: 'ByCategory', component: HomePage, props: true },
  { path: '/search/:search', name: 'BySearch', component: HomePage, props: true },
  { path: '/tag/:tag', name: 'ByTag', component: HomePage, props: true },

  { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFound },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
