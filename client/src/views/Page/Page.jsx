import React, { Component } from 'react';
import _ from 'lodash';
import { withRouter } from 'react-router-dom';
import Header from '../../components/Header/Header';
import Menu from '../../components/Menu/Menu';
import Button from '../../components/Page/Button';
import Modal from '../../components/Page/Modal';
import Table from '../../components/Page/Table';
import Info from '../../components/Info/Info.jsx';
import './Page.css';

class Page extends Component
{
    constructor(props)
    {
		super(props);

		this.state = {
            actionModalOpen: false,
            actionModalProps: {}
        };
        
        this.clickModalAction = this.clickModalAction.bind(this);
        this.clickNavigationButton = this.clickNavigationButton.bind(this);
    }

    clickModalAction(action, actionFields, title)
    {
        this.setState({
            actionModalOpen: true,
            actionModalProps: {
                action,
                actionFields,
                title
            }
        });
    }

    clickNavigationButton(navigateTo)
    {
        this.props.history.push(navigateTo);
    }

    /**
     * Returns function for button click event handling.
     * @param {object} button 
     */
    getButtonClick(button)
    {
        if (button.buttonType === 'ModalAction')
        {
            return this.clickModalAction;
        }

        if (button.buttonType === 'Navigation')
        {
            return this.clickNavigationButton;
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

                    case 'Field':
                        return <div key={`field-${i}`}>Insert field here</div>;

                    case 'Table':
                        return <Table key={`itemlist-${i}`} {...component} />;

                    default:
                        throw new Error(`Component type ${component.componentType} not implemented`);
                }
            })}
        </div>;
    }
    
    render()
    {
        const { state } = this;

        const pathname = window.location.pathname;
        // Parse page name from path and convert first char to upper
        let pageKey = pathname
            .substring(pathname.lastIndexOf('/') + 1);

        pageKey = pageKey
            .charAt(0)
            .toUpperCase() + pageKey.slice(1);

        return <div className="content">
            <Modal 
                onClose={() => 
                {
                    this.setState({ actionModalOpen: false });
                }} 
                page={pageKey}
                show={state.actionModalOpen} 
                {...state.actionModalProps}/>
            <Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disableValue={pageKey}></Menu>
            <Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
            {this.renderComponents()}
        </div>;
    }
};

export default withRouter(Page);