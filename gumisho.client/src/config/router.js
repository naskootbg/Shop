import { createRouter, createWebHistory } from "vue-router";
import HomePage from '../views/HomePage.vue';
import AdminPage from '../views/AdminPage.vue';
import LoginPage from '../views/LoginPage.vue';
import RegisterPage from '../views/RegisterPage.vue';
import { useUserStore } from '../stores/useUserStore';
import NotFound from '../views/NotFound.vue';
import Profile from '../views/Profile.vue';

const routes = [
    {path: '/', name:'home', component: HomePage},
    {path: '/admin', name:'admin', component: AdminPage},
    {path: '/login', name:'login', component: LoginPage,
      beforeEnter: async () => {
        const store = useUserStore();
        await store.onEnter();
        if (store.isUserLogged && store.isAdmin) {
          return '/admin'
        }
        else if (store.isUserLogged && !store.isAdmin) {
          return '/'
        }
      },
    },
    {path: '/join', name:'join', component: RegisterPage},
    {path: '/api/logout', name:'logout'},
    {path: '/profile', name:'profile', component: Profile },
    { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFound },
];

const router = createRouter({
    routes,
    history: createWebHistory(),
});

export default router;
