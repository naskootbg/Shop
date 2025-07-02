import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";


export async function LoadAll(hashes) {
  try {
    const res = await axiosUrl.post('feed/all', {
      hashes

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return res.data;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}


export async function AddHash(hash, name) {
  try {
    const res = await axiosUrl.post('feed/add', {
      hash,
      name
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}



export async function DelHash(id) {
  try {
    const res = await axiosUrl.post('feed/del?id=' + id, {}, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}
