import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";


export async function AddProduct(card) {
  try {
    const res = await axiosUrl.post('card/add-item', 
      card

    , {
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
export async function RemoveProduct(prod) {
  try {
    const res = await axiosUrl.post('card/del-item', {
      prod

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

export async function ClearCard(id) {
  try {
    const res = await axiosUrl.post('card/clear?id=' + id, {
      

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
export async function CreateCard(email, username) {
  try {
    const card = {
      email: email,
      username: username
    }
    const res = await axiosUrl.post('card/create', {
      card

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
export async function ShowCard(id) {
  try {
    const res = await axiosUrl.post('card/show?id=' + id, {


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
