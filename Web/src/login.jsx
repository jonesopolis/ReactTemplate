import React, { PropTypes } from 'react';
import { login } from 'api/account'
import Cookies from 'js-cookie';
import ReactDOM from 'react-dom';
import less from 'login/login.less'

class Login extends React.Component {

    constructor(props) {
        super(props);

        this.state = {
            username: 'david',
            password: 'david',
            errors: []
        }
    }

    login() {
        login(this.state.username, this.state.password)
            .then(e => {
                if (e.data.errors) {
                    this.setState({ errors: e.data.errors });
                } else {
                    Cookies.set("auth", e.data.token);
                    window.location.href = '/';
                }
            })
            .catch(e => {
                console.log(e);
                this.setState({ errors: 'Unknown error communicating with the service '});
            });
    }

    render() {

        let errors = null;
        if (this.state.errors.length > 0) {
            console.log(this.state.errors);
            errors = <div className='alert alert-danger'>{this.state.errors.map(e => { return (<div key={e}>{e}</div>); })}</div>;
        }

        return (
            <div className="centered-child">
                
                <div className="form-group">
                    <label>Email</label>
                    <input className="form-control" value={this.state.username} onChange={(e) => this.setState({
                        username: e.target.value
                    })}/>
                </div>

                <div className="form-group">
                    <label>Password</label>
                    <input className="form-control" value={this.state.password} onChange={(e) => this.setState({
                        password: e.target.value
                    })}/>
                </div>

                {errors}

                <button className="btn btn-lg btn-primary" onClick={() => this.login()}>Sign In</button>

            </div>
        );
    }
}

ReactDOM.render(<Login />, document.getElementById('login'));