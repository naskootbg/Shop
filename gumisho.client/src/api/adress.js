import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";

export async function ShowAdres() {
  try {
    const res = await axiosUrl.get('address/show');
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}

export async function AllAdresses() {
  try {
    const res = await axiosUrl.get('adresss/all');
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}

export async function AddAddress({ address, cityId, phone, stateId, fullName, latitude, longitude, userId, vhod, floor, appartanment }) {
  try {
    const res = await axiosUrl.post('address/add', {
      userId,
      address,
      cityId,
      phone,
      stateId,
      fullName,
      latitude,
      longitude, vhod, floor, appartanment

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status != 200) {

      return false;
    }
    console.log(res.data);
    return res.data
   
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function EditAddress({ id, address, cityId, phone, stateId, fullName, latitude, longitude, userId, vhod, floor, appartanment }) {
  try {
    const res = await axiosUrl.post('address/edit', {
      id,
      address,
      cityId,
      phone,
      stateId,
      fullName,
      latitude,
      longitude,
      userId, vhod: "", floor: "", appartanment: ""

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status = 200) {

      return true;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function DeleteAddress( id ) {
  try {
    const res = await axiosUrl.post('address/delete?id='+id, {
 
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status = 200) {

      return true;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}
