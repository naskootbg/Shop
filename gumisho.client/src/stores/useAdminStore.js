import { defineStore } from 'pinia';
import { LoadAll } from '@/api/feed';

export const useAdminStore = defineStore('adminStore', {
  state() {
    return { 
      hashes: [],
       
    };
  },
  getters: {
     
  },
  actions: {
    async onEnter() {

      this.hashes = await LoadAll();
      },
      
      
    },
    
   
  },
  
);
