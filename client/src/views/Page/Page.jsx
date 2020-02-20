import React, { Component } from 'react';
import _ from 'lodash';
import Header from '../../components/Header/Header';
import Menu from '../../components/Menu/Menu';
import Button from '../../components/Page/Button';
import './Page.css';

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

    renderComponents()
    {
        const { components } = this.props;

        return <div className='page-content'>
            {_.map(components, (component, i) => 
            {
                switch (component.componentType)
                {
                    case 'Button':
                        return <Button key={`button-${i}`} {...component} />;

                    case 'Input':
                        return <div key={`input-${i}`}>Insert input here</div>;

                    default:
                        throw new Error(`Component type ${component.componentType} not implemented`);
                }
            })}
        </div>;
    }
    
    render()
    {
        return <div className="content">
            <Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disable={this.state.menuDisabled}></Menu>
            {this.renderComponents()}
        </div>;
    }
};

export default Page;