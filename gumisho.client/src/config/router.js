import HomePage from '../views/HomePage.vue';
import AdminPage from '../views/AdminPage.vue';
import LoginPage from '../views/LoginPage.vue';
import RegisterPage from '../views/RegisterPage.vue';
import NotFound from '../views/NotFound.vue';
import Profile from '../views/Profile.vue';
import ProductDetails from '@/views/ProductDetails.vue';
import { useUserStore } from '../stores/useUserStore';

export default [
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

  { path: '/:id/:slug', name: 'ProductDetails', component: ProductDetails, props: true },

  // Filter routes → all render HomePage
  { path: '/discount/:value', name: 'ByDiscount', component: HomePage, props: true },
  { path: '/category/:category', name: 'ByCategory', component: HomePage, props: true },
  { path: '/search/:search', name: 'BySearch', component: HomePage, props: true },
  { path: '/tag/:tag', name: 'ByTag', component: HomePage, props: true },

  { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFound },
];
