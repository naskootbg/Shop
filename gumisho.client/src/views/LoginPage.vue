<script>
import { reactive } from 'vue';
import useVuelidate from '@vuelidate/core';
import { required } from '@vuelidate/validators';
import { useUserStore } from '../stores/useUserStore';
  import FormFieldset from '@/components/FormFieldset.vue';
import  { formDataBG } from '@/const';
 


export default {

    components: {
        FormFieldset,
     
    },

    setup() {
        const userStore = useUserStore();



        // Form state
        const form = reactive({
          username: 'admin@evtinoo.com',
            password: 'aaaaaa',
        });


        // Validations
        const rules = {

            username: { required },
            password: { required },

        };

        const v$ = useVuelidate(rules, form);

        // Login method
        const onLogin = async () => {
            const isValid = await v$.value.$validate();
            if (!isValid) return;

            try {

                 await userStore.LoginIn(form);

            } catch (error) {
                console.error('Login failed:', error);
            }
        };



        return {
            formDataBG,
            form,
            v$,
            onLogin,
        };
    },
};
</script>


<template>
 
    <div class="formContainer">
        <article>
            <form @submit.prevent="onLogin">
                <FormFieldset :title="formDataBG.username" :errors="v$.username.$errors">
                    <input v-model="form.username" type="text" :placeholder= "formDataBG.username" />
                </FormFieldset>

                <FormFieldset :title="formDataBG.password" :errors="v$.password.$errors">
                    <input v-model="form.password" type="password" :placeholder="formDataBG.password" />
                </FormFieldset>

                <button class="primary" type="submit">Login</button>
            </form>
        </article>
    </div>
   
</template>








<style scoped>
.formContainer {
    padding: 1rem;
    display: flex;
    justify-content: center;
    align-items: center;
}

.formContainer article {
    width: 100%;
    max-width: 500px;
}
</style>
