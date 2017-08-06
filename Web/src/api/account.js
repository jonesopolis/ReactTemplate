import axios from 'axios'

export const login = (username, password) => {
    return axios({
        method: 'post',
        url: 'api/account/login',
        data: {
            Username: username,
            Password: password
        }
    });
}