import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";

export async function ShowService(id){
    try{
const res = await axiosUrl.get('service/show?id='+id);
return res.data;
    }
    catch (e){
logger.error(e);
return null;
    }  
}

export async function AllServices(){
    try{
const res = await axiosUrl.get('service/all');
return res.data;
    }
    catch (e){
logger.error(e);
return null;
    }  
}

export async function RemoveService({id}){
    try{
const res = await axiosUrl.post('service/delete?id='+id,{
     
},{
    headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
        
      },
});
if(res.status = 200){
 
return true;
}

return false;
    }
    catch (e){
logger.error(e);
return[];
    }
}

export async function AddService({name, description, priceCarpet, priceDelivery}){
    try{
const res = await axiosUrl.post('service/add',{
    name,
    description,
    priceCarpet,
    priceDelivery
},{
    headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
        
      },
});
if(res.status = 200){
 
return true;
}

return false;
    }
    catch (e){
logger.error(e);
return[];
    }
}

export async function EditService({id, name, description, priceCarpet, priceDelivery}){
    try{
const res = await axiosUrl.post('service/edit',{
    id,
    name,
    description,
    priceCarpet,
    priceDelivery
},{
    headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
        
      },
});
if(res.status = 200){
 
return true;
}

return false;
    }
    catch (e){
logger.error(e);
return[];
    }
}
