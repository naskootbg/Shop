import axios from "axios";

const axiosUrl = axios.create({
  baseURL: "/api/"
})


export default axiosUrl;
