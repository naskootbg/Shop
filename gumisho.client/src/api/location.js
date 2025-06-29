import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";
import axiosGeoapify from "@/config/axiosGeo";
import axiosG from "@/config/axiosG";

export async function SuggestAddress(q) {
  try {
    const res = await axiosGeoapify.get('?text=' + q + "&format=json&apiKey=456d9e9e450e41f09bd2354d7b259b3a");
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}

export async function GetCountry(id){
    try{
const res = await axiosUrl.get('location/country?id='+id);
return res.data;
    }
    catch (e){
logger.error(e);
return null;
    }  
}

//export async function GetState(id){
//    try{
//const res = await axiosUrl.get('location/state?id='+id);
//return res.data;
//    }
//    catch (e){
//logger.error(e);
//return null;
//    }  
//}
export async function GetState() {
  try {
    const res = await axiosUrl.get(`Location/all-states`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}
export async function GetCity(id){
    try{
      const res = await axiosUrl.get('Location/city?id='+id);
return res.data;
    }
    catch (e){
logger.error(e);
return null;
    }  
}

export async function GetCityName(id) {
  try {
    const res = await axiosUrl.get('Location/city-name?id=' + id);
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}

export async function GetStateName(id) {
  try {
    const res = await axiosUrl.get('Location/state-name?id=' + id);
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}

export async function AzureMap(q) {
  try {
    const res = await axiosG.get("?api-version=2023-06-01&addressLine=" + q.toLowerCase() + "&countryRegion=BG&subscription-key=Ccl9LpOH53RhEzcjg4mwGx4YuUJKn9XElWhXoky0Gq3qoL9Vc95pJQQJ99BAACYeBjFVjKMpAAAgAZMP4Hrt");
    //  //=%D0%B1%D0%B5%D0%BB%D0%BE%D0%BC%D0%BE%D1%80%D1%81%D0%BA%D0%B0%2015%20%D0%BA%D1%80%D1%83%D0%BC%D0%BE%D0%B2%D0%B3%D1%80%D0%B0%D0%B4&countryRegion=BG&subscription-key=Ccl9LpOH53RhEzcjg4mwGx4YuUJKn9XElWhXoky0Gq3qoL9Vc95pJQQJ99BAACYeBjFVjKMpAAAgAZMP4Hrt
   // console.log(res.data.features.length);
     
    return res.data.features;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}

