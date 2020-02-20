import React, { Component } from 'react';
import _ from 'lodash';
import Header from '../../components/Header/Header';
import Menu from '../../components/Menu/Menu';
import Button from '../../components/Page/Button';
import Modal from '../../components/Page/Modal';
import './Page.css';

class Page extends Component
{
    constructor(props)
    {
		super(props);

		this.state = {
            // TODO: Toteuta toisella tapaa. Ei toimi heti kun lisätään uusia sivuja
            menuDisabled: [false, false, false, false, true, false],
            actionModalOpen: false,
            actionModalProps: {}
        };
        
        this.clickModalAction = this.clickModalAction.bind(this);
    }

    clickModalAction(actionUrl, actionFields)
    {
        this.setState({
            actionModalOpen: true,
            actionModalProps: {
                actionUrl,
                actionFields
            }
        });
    }

    /**
     * Returns function for button click event handling.
     * @param {object} button 
     */
    getButtonClick(button)
    {
        console.log(button);
        if (button.buttonType === 'ModalAction')
        {
            return this.clickModalAction;
        }

        return null;
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
                        return <Button 
                            key={`button-${i}`}
                            onClick={this.getButtonClick(component)} 
                            {...component} />;

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
        console.log(this.state);
        return <div className="content">
            <Modal 
                onClose={() => 
                {
                    this.setState({ actionModalOpen: false });
                }} 
                show={this.state.actionModalOpen} 
                {...this.state.actionModalProps}/>
            <Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disable={this.state.menuDisabled}></Menu>
            {this.renderComponents()}
        </div>;
    }
};

export default Page;