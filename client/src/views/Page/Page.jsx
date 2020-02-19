import React, { Component } from 'react';
import Header from '../../components/Header/Header.jsx';
import Menu from '../../components/Menu/Menu.jsx';

class Page extends Component
{
    constructor(props)
    {
		super(props);

		this.state = {
            // TODO: Toteuta toisella tapaa. Ei toimi heti kun lisätään uusia sivuja
			menuDisabled: [false, false, false, false, true, false]
		};
    }
    
    render()
    {
        return <div className="content">
            <Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disable={this.state.menuDisabled}></Menu>
            <div>Page view</div>
        </div>;
    }
};

export default Page;